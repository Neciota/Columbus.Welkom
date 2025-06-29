namespace Columbus.Welkom.Application.Models.Entities
{
    public class SelectedYoungPigeonEntity : IEntity
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int PigeonId { get; set; }

        public OwnerEntity? Owner { get; set; }
        public PigeonEntity? Pigeon { get; set; }
    }
}
