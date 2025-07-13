using Columbus.Models.Owner;
using Columbus.Models.Pigeon;

namespace Columbus.Welkom.Application.Models.Entities
{
    public class SelectedYoungPigeonEntity : IEntity
    {
        private CountryCode _countryCode;
        private int _year;
        private RingNumber _ringNumber;

        public int Id { get; set; }
        public OwnerId OwnerId { get; set; }
        public PigeonId PigeonId
        {
            get => PigeonId.Create(_countryCode, _year, _ringNumber);
            set
            {
                _countryCode = value.CountryCode;
                _year = value.Year;
                _ringNumber = value.RingNumber;
            }
        }

        public OwnerEntity? Owner { get; set; }
        public PigeonEntity? Pigeon { get; set; }
    }
}
