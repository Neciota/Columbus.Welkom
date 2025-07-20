using Columbus.Models.Owner;
using Columbus.Models.Pigeon;

namespace Columbus.Welkom.Application.Models.ViewModels
{
    public class OwnerPigeonPair
    {
        public OwnerPigeonPair()
        {

        }

        public OwnerPigeonPair(Owner owner, Pigeon pigeon)
        {
            Owner = owner;
            Pigeon = pigeon;
        }

        public OwnerPigeonPair(Owner owner, Pigeon pigeon, int points)
        {
            Owner = owner;
            Pigeon = pigeon;
            Points = points;
        }

        public int Id { get; set; }
        public Owner? Owner { get; set; }
        public Pigeon? Pigeon { get; set; }
        public double Points { get; set; }

        public void ResetOnOwnerChange()
        {
            Pigeon = null;
            Points = 0;
        }
    }
}
