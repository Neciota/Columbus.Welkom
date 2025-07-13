using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Models.Race;

namespace Columbus.Welkom.Application.Models.Entities
{
    public class PigeonRaceEntity() : IEntity
    {
        private CountryCode _countryCode;
        private int _year;
        private RingNumber _ringNumber;

        public PigeonRaceEntity(PigeonRace pigeonRace, string raceCode) : this()
        {
            PigeonId = pigeonRace.Pigeon.Id;
            RaceCode = raceCode;
            Mark = pigeonRace.Mark;
            ArrivalTime = pigeonRace.ArrivalTime;
        }

        public string RaceCode { get; set; } = string.Empty;
        public PigeonId PigeonId
        {
            get => PigeonId.Create(_countryCode, _year, _ringNumber);
            set {
                _countryCode = value.CountryCode;
                _year = value.Year;
                _ringNumber = value.RingNumber;
            }
        }
        public int Mark { get; set; }
        public DateTime? ArrivalTime { get; set; }

        public PigeonEntity? Pigeon { get; set; }
        public RaceEntity? Race { get; set; }

        public PigeonRace ToPigeonRace(OwnerId ownerId)
        {
            if (Pigeon is null)
                throw new InvalidOperationException("Pigeon cannot be null");

            return new PigeonRace(Pigeon.ToPigeon(), ownerId, ArrivalTime, Mark);
        }
    }
}
