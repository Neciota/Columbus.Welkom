using Columbus.Models;
using Columbus.Models.Owner;

namespace Columbus.Welkom.Application.Models.Entities
{
    public class OwnerEntity() : IEntity
    {
        public OwnerEntity(Owner owner) : this()
        {
            Name = owner.Name;
            Latitude = owner.LoftCoordinate.Lattitude;
            Longitude = owner.LoftCoordinate.Longitude;
            Club = owner.Club;
            OwnerId = owner.Id;
        }

        public OwnerId OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public ClubId Club { get; set; }

        public ICollection<PigeonEntity> Pigeons { get; set; } = [];
        public LeagueOwnerEntity? LeagueOwner { get; set; }
        public OwnerTeamEntity? OwnerTeam { get; set; }
        public SelectedYearPigeonEntity? SelectedYearPigeon { get; set; }
        public SelectedYoungPigeonEntity? SelectedYoungPigeon { get; set; }
        public ICollection<PigeonSaleEntity> PigeonSale { get; set; } = [];
        public ICollection<PigeonSaleEntity> PigeonBuy { get; set; } = [];
        public ICollection<PigeonSwapEntity> PlayedSwappedPigeons { get; set; } = [];
        public ICollection<PigeonSwapEntity> OwnedSwappedPigeons { get; set; } = [];
        public ICollection<PigeonSwapEntity> CoupledSwappedPigeons { get; set; } = [];

        public Owner ToOwner() => new(OwnerId, Name, new Coordinate(Longitude, Latitude), Club, Pigeons.Select(p => p.ToPigeon()).ToList());
    }
}
