using Columbus.Models;
using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Models.Race;
using Columbus.Welkom.Application.Venira.Rows;
using System.Globalization;

namespace Columbus.Welkom.Application.Venira;

/// <inheritdoc cref="IVeniraRaceProvider"/>
public class VeniraRaceProvider(IVeniraReader reader) : IVeniraRaceProvider
{
    public async Task<IEnumerable<Race>> GetRacesAsync(int year, ClubId club)
    {
        IReadOnlyList<VeniraFlightRow> flights = await reader.GetFlightsAsync();
        IReadOnlyList<VeniraReleaseSiteRow> releaseSites = await reader.GetReleaseSitesAsync();
        IReadOnlyList<VeniraOwnerRow> owners = await reader.GetOwnersAsync();
        IReadOnlyList<VeniraArrivalRow> arrivals = await reader.GetArrivalsAsync();
        IReadOnlyList<VeniraClockRow> clocks = await reader.GetClocksAsync();

        // Vlucht.Lossingsplaats stores the place name rather than a key into Losplaat.
        Dictionary<string, Coordinate> locationsByReleaseSite = releaseSites
            .Where(s => s.Name is not null)
            .GroupBy(s => s.Name!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => VeniraValues.ParseCoordinate(g.First().Latitude, g.First().Longitude),
                StringComparer.OrdinalIgnoreCase);

        Dictionary<OwnerId, Owner> ownersById = owners.Select(ToOwner)
            .OfType<Owner>()
            .ToDictionary(o => o.Id);

        ILookup<int, VeniraArrivalRow> arrivalsByFlight = arrivals.ToLookup(a => a.FlightId);

        // Klok.DB is keyed on flight and member, so one row per pair.
        Dictionary<(int Flight, OwnerId Owner), VeniraClockRow> clocksByFlightAndOwner = [];
        foreach (VeniraClockRow clock in clocks)
        {
            if (OwnerId.TryParse(clock.MemberNumber, CultureInfo.InvariantCulture, out OwnerId owner))
                clocksByFlightAndOwner[(clock.FlightId, owner)] = clock;
        }

        return flights.Where(f => f.HasResults && f.ReleaseDate!.Value.Year == year)
            .Select(f => BuildRace(f, club, locationsByReleaseSite, ownersById, arrivalsByFlight, clocksByFlightAndOwner))
            .OfType<Race>()
            .ToList();
    }

    private static Race? BuildRace(
        VeniraFlightRow flight,
        ClubId club,
        IReadOnlyDictionary<string, Coordinate> locationsByReleaseSite,
        IReadOnlyDictionary<OwnerId, Owner> ownersById,
        ILookup<int, VeniraArrivalRow> arrivalsByFlight,
        IReadOnlyDictionary<(int Flight, OwnerId Owner), VeniraClockRow> clocksByFlightAndOwner)
    {
        if (!RaceType.TryParse(flight.RaceTypeCode, CultureInfo.InvariantCulture, out RaceType raceType))
            return null;

        if (flight.Number is not short number)
            return null;

        if (VeniraValues.Combine(flight.ReleaseDate, flight.ReleaseTime) is not DateTime startTime)
            return null;

        Coordinate location = default;
        if (flight.ReleaseSite is not null)
            locationsByReleaseSite.TryGetValue(flight.ReleaseSite, out location);

        List<PigeonRace> pigeonRaces = [];
        Dictionary<OwnerId, int> pigeonCountsByOwner = [];

        foreach (VeniraArrivalRow arrival in arrivalsByFlight[flight.FlightId])
        {
            if (!OwnerId.TryParse(arrival.MemberNumber, CultureInfo.InvariantCulture, out OwnerId ownerId) || !ownerId.HasClubId(club))
                continue;

            if (VeniraValues.ParsePigeonId(arrival.CountryCode, arrival.RingNumber) is not PigeonId pigeonId)
                continue;

            Pigeon pigeon = new(pigeonId, VeniraValues.ParseChip(arrival.Chip), VeniraValues.ParseSex(arrival.Sex));

            PigeonRace pigeonRace = new(
                pigeon,
                ownerId,
                ResolveArrival(arrival, startTime),
                arrival.Mark ?? 0,
                arrival.OwnerArrivalOrder);

            // Correct the arrival here rather than once per owner: an owner may run several
            // clocks in one flight, each with its own drift, while OwnerRace carries a single
            // deviation. This also mirrors what is stored, since PigeonRaceEntity persists the
            // corrected time and neither the deviation nor the clock times.
            VeniraClockReading? reading = GetClockReading(flight.FlightId, ownerId, arrival, clocksByFlightAndOwner);
            pigeonRace.ArrivalTime = pigeonRace.GetCorrectedArrivalTime(
                reading?.SubmissionAt,
                reading?.StoppedAt,
                reading?.Deviation ?? TimeSpan.Zero);

            pigeonRaces.Add(pigeonRace);
            pigeonCountsByOwner[ownerId] = pigeonCountsByOwner.GetValueOrDefault(ownerId) + 1;
        }

        if (pigeonRaces.Count == 0)
            return null;

        List<OwnerRace> ownerRaces = [.. pigeonCountsByOwner.Select(oc => new OwnerRace(
            ownersById.GetValueOrDefault(oc.Key) ?? new Owner(oc.Key, string.Empty, default, club),
            location,
            oc.Value,
            submissionAt: null,
            stoppedAt: null,
            clockDeviation: TimeSpan.Zero))];

        // The no-points constructor: ranking happens on read, in RaceEntity.ToRace, once the
        // points settings for this race type are known. Ordering the pigeons here would be
        // wasted work for the same reason.
        return new Race(
            number,
            raceType,
            flight.ReleaseSite ?? string.Empty,
            BuildCode(raceType, number),
            startTime,
            location,
            ownerRaces,
            pigeonRaces);
    }

    /// <summary>
    /// The moment the pigeon was clocked, on its owner's clock.
    /// </summary>
    /// <remarks>
    /// Venira fills <c>Constateringsdatum</c> only for arrivals read from an ECS clock; an
    /// arrival typed in by hand gets a time of day and no date at all. Those fall back to the
    /// day of the release, which is what Venira itself scores them against, and roll over to the
    /// next day if the time of day precedes the release.
    /// </remarks>
    private static DateTime? ResolveArrival(VeniraArrivalRow arrival, DateTime startTime)
    {
        if (!arrival.HasArrived || arrival.ArrivalTime is not TimeSpan timeOfDay)
            return null;

        if (arrival.ArrivalDate is not null)
            return VeniraValues.Combine(arrival.ArrivalDate, timeOfDay);

        DateTime arrivalOnReleaseDay = DateTime.SpecifyKind(startTime.Date.Add(timeOfDay), DateTimeKind.Local);

        return arrivalOnReleaseDay < startTime ? arrivalOnReleaseDay.AddDays(1) : arrivalOnReleaseDay;
    }

    private static VeniraClockReading? GetClockReading(
        int flightId,
        OwnerId ownerId,
        VeniraArrivalRow arrival,
        IReadOnlyDictionary<(int Flight, OwnerId Owner), VeniraClockRow> clocksByFlightAndOwner)
    {
        if (!arrival.HasArrived || arrival.ClockIndex is not short clockNumber)
            return null;

        return clocksByFlightAndOwner.TryGetValue((flightId, ownerId), out VeniraClockRow? clockRow)
            ? clockRow.GetReading(clockNumber)
            : null;
    }

    /// <summary>
    /// The race's key, e.g. <c>V07</c> for Vitesse number 7. Venira numbers flights
    /// sequentially across the whole season regardless of race type, so this is unique within a
    /// year — and each database already covers a single club and year.
    /// </summary>
    private static string BuildCode(RaceType raceType, short number) =>
        string.Create(CultureInfo.InvariantCulture, $"{raceType}{number:00}");

    private static Owner? ToOwner(VeniraOwnerRow row)
    {
        if (!OwnerId.TryParse(row.MemberNumber, CultureInfo.InvariantCulture, out OwnerId id))
            return null;

        if (!ClubId.TryParse(row.ClubNumber, CultureInfo.InvariantCulture, out ClubId club))
            return null;

        return new Owner(id, row.Name ?? string.Empty, VeniraValues.ParseCoordinate(row.Latitude, row.Longitude), club);
    }
}
