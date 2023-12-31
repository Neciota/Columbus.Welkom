﻿using Columbus.Models;

namespace Columbus.Welkom.Client.Models
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

        public Owner? Owner { get; set; }
        public Pigeon? Pigeon { get; set; }
        public int Points { get; set; }

        public void ResetOnOwnerChange()
        {
            Pigeon = null;
            Points = 0;
        }
    }
}
