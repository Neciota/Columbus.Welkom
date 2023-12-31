﻿using Columbus.Models;

namespace Columbus.Welkom.Client.Models.Entities
{
    public class PigeonRaceEntity : BaseEntity
    {
        public PigeonRaceEntity() { }

        public PigeonRaceEntity(PigeonRace pigeonRace, PigeonEntity pigeon, RaceEntity race)
        {
            Pigeon = pigeon;
            Race = race;
            Mark = pigeonRace.Mark;
            ArrivalTime = pigeonRace.ArrivalTime;
        }

        public PigeonRaceEntity(PigeonRace pigeonRace, int pigeonId, int raceId)
        {
            PigeonId = pigeonId;
            RaceId = raceId;
            Mark = pigeonRace.Mark;
            ArrivalTime = pigeonRace.ArrivalTime;
        }

        public int PigeonId { get; set; }
        public int RaceId { get; set; }
        public int Mark { get; set; }
        public DateTime ArrivalTime { get; set; }

        public PigeonEntity? Pigeon { get; set; }
        public RaceEntity? Race { get; set; }

        public PigeonRace ToPigeonRace()
        {
            if (Pigeon is null)
                throw new ArgumentNullException("Pigeon cannot be null");

            return new PigeonRace(Pigeon.ToPigeon(), ArrivalTime, Mark);
        }
    }
}
