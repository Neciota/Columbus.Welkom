using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Models.Entities
{
    public class PigeonSwapEntity : IEntity
    {
        private CountryCode _countryCode;
        private int _year;
        private RingNumber _ringNumber;

        public int Id { get; set; }
        public OwnerId OwnerId { get; set; }
        public OwnerId PlayerId { get; set; }
        public OwnerId CoupledPlayerId { get; set; }
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

        public OwnerEntity? Player { get; set; }
        public OwnerEntity? Owner { get; set; }
        public PigeonEntity? Pigeon { get; set; }
        public OwnerEntity? CoupledPlayer { get; set; }

        public PigeonSwapPair ToPigeonSwapPair()
        {
            if (Player is null || Owner is null || Pigeon is null || CoupledPlayer is null)
                throw new InvalidOperationException("One or more of the related entities is not set.");

            return new PigeonSwapPair(Player.ToOwner(), Owner.ToOwner(), Pigeon.ToPigeon(), CoupledPlayer.ToOwner());
        }
    }
}
