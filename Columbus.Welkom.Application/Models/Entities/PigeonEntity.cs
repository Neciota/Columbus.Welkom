using Columbus.Models.Owner;
using Columbus.Models.Pigeon;

namespace Columbus.Welkom.Application.Models.Entities
{
    public class PigeonEntity : IEntity
    {
        private CountryCode _countryCode;
        private int _year;
        private RingNumber _ringNumber;

        public PigeonEntity() { }

        public PigeonEntity(Pigeon pigeon, OwnerId ownerId)
        {
            Id = pigeon.Id;
            Chip = pigeon.Chip;
            Sex = pigeon.Sex;
            OwnerId = ownerId;
        }

        public PigeonId Id
        {
            get => PigeonId.Create(_countryCode, _year, _ringNumber);
            set
            {
                _countryCode = value.CountryCode;
                _year = value.Year;
                _ringNumber = value.RingNumber;
            }
        }
        public int Chip { get; set; }
        public Sex Sex { get; set; }
        public OwnerId OwnerId { get; set; }

        public OwnerEntity? Owner { get; set; }
        public ICollection<PigeonRaceEntity>? PigeonRaces { get; set; }
        public SelectedYearPigeonEntity? SelectedYearPigeonEntity { get; set; }
        public SelectedYoungPigeonEntity? SelectedYoungPigeonEntity { get; set; }
        public PigeonSaleEntity? PigeonSale { get; set; }
        public ICollection<PigeonSwapEntity> PigeonSwaps { get; set; } = [];

        public Pigeon ToPigeon() => new Pigeon(Id, Chip, Sex);

        public override bool Equals(object? obj) => obj is PigeonEntity pigeon && Equals(pigeon);
        public bool Equals(Pigeon pigeon) => Id == pigeon.Id;

        /// <summary>
        /// Overriden to only include country, year, and ringnumber.
        /// This makes comparison between entities easier, especially using hashsets for performant inclusion checks.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(Id);
            return hashCode.ToHashCode();
        }
    }
}
