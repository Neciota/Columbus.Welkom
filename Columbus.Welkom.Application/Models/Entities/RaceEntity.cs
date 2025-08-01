﻿using Columbus.Models;
using Columbus.Models.Owner;
using Columbus.Models.Race;
using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Models.Entities
{
    public class RaceEntity : IEntity
    {
        public RaceEntity() { }

        public RaceEntity(Race race)
        {
            Number = race.Number ?? 0;
            Code = race.Code;
            Name = race.Name;
            Type = race.Type;
            StartTime = race.StartTime;
            Latitude = race.Location.Lattitude;
            Longitude = race.Location.Longitude;
        }

        public string Code { get; set; } = string.Empty;
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public RaceType Type { get; set; }
        public DateTime StartTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public ICollection<PigeonRaceEntity> PigeonRaces { get; set; } = [];

        public Race ToRace(int pointsQuotient, int maxPoints, int minPoints, int decimalPlaces, INeutralizationTime neutralizationTime)
        {
            if (PigeonRaces.Any(pr => pr.Pigeon is null))
                throw new InvalidOperationException("Pigeon on PigeonProperty cannot be null");
            if (PigeonRaces.Any(pr => pr.Pigeon!.Owner is null))
                throw new InvalidOperationException("Owner on Pigeon on PigeonProperty cannot be null");

            Coordinate startLocation = new Coordinate(Longitude, Latitude);

            Dictionary<OwnerId, OwnerRace> ownerRaces = PigeonRaces.Select(pr => pr.Pigeon!.Owner)
                .Distinct()
                .Cast<OwnerEntity>()
                .Select(o => new OwnerRace(o.ToOwner(), startLocation, PigeonRaces.Count(pr => pr.Pigeon!.OwnerId == o.OwnerId), null, null, TimeSpan.Zero))
                .ToDictionary(or => or.Owner.Id);
            IList<PigeonRace> pigeonRaces = PigeonRaces.Select(pr => pr.ToPigeonRace(pr.Pigeon!.Owner!.OwnerId))
                .OrderByDescending(pr => pr.GetSpeed(ownerRaces[pr.OwnerId].Distance, StartTime, null, null, ownerRaces[pr.OwnerId].ClockDeviation, neutralizationTime))
                .ThenBy(pr => pr.ArrivalOrder)
                .ToList();

            return new Race(
                Number,
                Type,
                Name,
                Code,
                StartTime,
                startLocation,
                ownerRaces.Values.ToList(),
                pigeonRaces,
                pointsQuotient,
                maxPoints,
                minPoints,
                decimalPlaces);
        }

        public SimpleRace ToSimpleRace()
        {
            if (PigeonRaces is null)
                throw new InvalidOperationException("PigeonRaces cannot be null");

            Coordinate startLocation = new Coordinate(Longitude, Latitude);

            int ownerRaceCount = PigeonRaces.Select(pr => pr.Pigeon!.Owner)
                .Distinct()
                .Count();

            return new SimpleRace(Number, Type, Name, Code, StartTime, startLocation, ownerRaceCount, PigeonRaces.Count);
        }
    }
}
