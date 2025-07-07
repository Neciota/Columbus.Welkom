using Columbus.Models;
using Columbus.Models.Owner;

namespace Columbus.Welkom.Application.Models.Entities
{
    public class OwnerEntity : IEntity
    {
        public OwnerEntity() { }

        public OwnerEntity(Owner owner)
        {
            Name = owner.Name;
            Latitude = owner.LoftCoordinate.Lattitude;
            Longitude = owner.LoftCoordinate.Longitude;
            Club = owner.Club;
            OwnerId = owner.Id;
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public ClubId Club { get; set; }
        public OwnerId OwnerId { get; set; }

        public ICollection<PigeonEntity>? Pigeons { get; set; }

        public Owner ToOwner() => new Owner(OwnerId.Create(Id), Name, new Coordinate(Longitude, Latitude), Club);
    }
}
