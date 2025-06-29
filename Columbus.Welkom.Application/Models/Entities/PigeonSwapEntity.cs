namespace Columbus.Welkom.Application.Models.Entities
{
    public class PigeonSwapEntity : IEntity
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int PlayerId { get; set; }
        public int OwnerId { get; set; }
        public int PigeonId { get; set; }
        public int CoupledPlayerId { get; set; }

        public OwnerEntity? Player { get; set; }
        public OwnerEntity? Owner { get; set; }
        public PigeonEntity? Pigeon { get; set; }
        public OwnerEntity? CoupledPlayer { get; set; }

        public PigeonSwapPair ToPigeonSwapPair()
        {
            if (Player is null || Owner is null || Pigeon is null || CoupledPlayer is null)
                throw new InvalidOperationException("One or more of the related entities is not set.");

            return new PigeonSwapPair(Id, Player.ToOwner(), Owner.ToOwner(), Pigeon.ToPigeon(), CoupledPlayer.ToOwner());
        }
    }
}
