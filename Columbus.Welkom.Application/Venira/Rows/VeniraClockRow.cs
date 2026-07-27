using PdxLib.Connector;
using System.Globalization;

namespace Columbus.Welkom.Application.Venira.Rows;

/// <summary>
/// One owner's clock readings for one flight, from Venira's <c>Klok.DB</c>. The table stores up
/// to ten clocks side by side as numbered column families (<c>Aand1</c>…<c>Aand10</c>), and
/// each arrival in <c>Aankomst.DB</c> names the clock that recorded it.
/// </summary>
public sealed record VeniraClockRow(
    int FlightId,
    string? MemberNumber,
    IReadOnlyList<VeniraClockReading> Readings)
{
    private const int MaxClocks = 10;

    /// <summary>
    /// The reading for a one-based clock number, or <see langword="null"/> when the flight has
    /// no such clock. Arrivals that were never clocked carry clock 0.
    /// </summary>
    public VeniraClockReading? GetReading(int clockNumber) =>
        clockNumber >= 1 && clockNumber <= Readings.Count ? Readings[clockNumber - 1] : null;

    public static VeniraClockRow FromRecord(ParadoxRecord record)
    {
        VeniraClockReading[] readings = new VeniraClockReading[MaxClocks];
        for (int clock = 1; clock <= MaxClocks; clock++)
        {
            string suffix = clock.ToString(CultureInfo.InvariantCulture);

            readings[clock - 1] = new VeniraClockReading(
                // Aanslag: the clock is sealed and synchronised at basketing.
                SubmissionDate: record.GetDate($"Aand{suffix}"),
                SubmissionTime: record.GetTime($"Aant{suffix}"),
                // Afslag: the clock is stopped and read against the master clock.
                StopDate: record.GetDate($"Afd{suffix}"),
                StopTime: record.GetTime($"Aft{suffix}"),
                MasterStopTime: record.GetTime($"Maft{suffix}"));
        }

        return new VeniraClockRow(
            FlightId: record.GetInt("VluchtID") ?? 0,
            MemberNumber: record.GetText("NPOlidnr"),
            Readings: readings);
    }
}

/// <summary>
/// A single clock's basketing and stopping times. All times are the owner's own clock, except
/// <see cref="MasterStopTime"/>, which is the master clock at the same moment.
/// </summary>
public sealed record VeniraClockReading(
    DateTime? SubmissionDate,
    TimeSpan? SubmissionTime,
    DateTime? StopDate,
    TimeSpan? StopTime,
    TimeSpan? MasterStopTime)
{
    private static readonly TimeSpan Day = TimeSpan.FromDays(1);
    private static readonly TimeSpan HalfDay = TimeSpan.FromHours(12);

    /// <summary>When the clock was synchronised and sealed, on the owner's clock.</summary>
    public DateTime? SubmissionAt => VeniraValues.Combine(SubmissionDate, SubmissionTime);

    /// <summary>When the clock was stopped, on the owner's clock.</summary>
    public DateTime? StoppedAt => VeniraValues.Combine(StopDate, StopTime);

    /// <summary>
    /// How far the master clock ran ahead of the owner's clock by the time it was stopped.
    /// Positive means the owner's clock lagged behind, which is the sign convention
    /// <c>OwnerRace.ClockDeviation</c> and <c>PigeonRace.GetCorrectedArrivalTime</c> expect.
    /// </summary>
    /// <remarks>
    /// Venira never fills the master clock's <em>date</em> columns, only its time of day, so a
    /// clock stopped either side of midnight from the master would otherwise read as a
    /// near-24-hour drift. Anything beyond half a day is therefore folded back.
    /// </remarks>
    public TimeSpan Deviation
    {
        get
        {
            if (StopTime is null || MasterStopTime is null)
                return TimeSpan.Zero;

            TimeSpan deviation = MasterStopTime.Value - StopTime.Value;

            if (deviation > HalfDay)
                return deviation - Day;

            if (deviation < -HalfDay)
                return deviation + Day;

            return deviation;
        }
    }
}
