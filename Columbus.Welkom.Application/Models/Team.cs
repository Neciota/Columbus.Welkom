using Columbus.Models;
using Columbus.Models.Owner;

namespace Columbus.Welkom.Application.Models
{
    public class Team
    {
        public Team()
        {

        }

        public Team(Owner firstOwner, Owner secondOwner, Owner thirdOwner)
        {
            FirstOwner = firstOwner;
            SecondOwner = secondOwner;
            ThirdOwner = thirdOwner;
        }

        public Owner? FirstOwner { get; set; }
        public Owner? SecondOwner { get; set; }
        public Owner? ThirdOwner { get; set; }
        public int FirstOwnerPoints { get; set; }
        public int SecondOwnerPoints { get; set; }
        public int ThirdOwnerPoints { get; set; }

        public int TotalPoints => FirstOwnerPoints + SecondOwnerPoints + ThirdOwnerPoints;

        public bool OwnerIsInTeam(Owner owner) => FirstOwner?.Id == owner.Id || SecondOwner?.Id == owner.Id || ThirdOwner?.Id == owner.Id;
    }
}
