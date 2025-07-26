using Columbus.Models.Owner;
using Columbus.Models.Pigeon;

namespace Columbus.Welkom.Application.Models.ViewModels
{
    public class PigeonSwapPair
    {
        public PigeonSwapPair()
        {
            RacePoints = new Dictionary<SimpleRace, int>();
        }

        public PigeonSwapPair(Owner player, Owner owner, Pigeon pigeon, Owner coupledOwner)
        {
            Player = player;
            Owner = owner;
            Pigeon = pigeon;
            CoupledPlayer = coupledOwner;
            RacePoints = new Dictionary<SimpleRace, int>();
        }

        public PigeonSwapPair(Owner player, Owner owner, Pigeon pigeon, Owner coupledOwner, Dictionary<SimpleRace, int> racePoints)
        {
            Player = player;
            Owner = owner;
            Pigeon = pigeon;
            CoupledPlayer = coupledOwner;
            RacePoints = racePoints;
        }
        
        public int Id { get; set; }
        public Owner Player { get; set; }
        public Owner Owner { get; set; }
        public Pigeon Pigeon { get; set; }
        public Owner CoupledPlayer { get; set; }
        public Dictionary<SimpleRace, int>? RacePoints { get; set; }
        public int Points => RacePoints?.Sum(rp => rp.Value) ?? 0;
    }
}
