﻿using Columbus.Models;
using Columbus.Welkom.Client.Models;

namespace Columbus.Welkom.Client.Services.Interfaces
{
    public interface ISelectedYoungPigeonService
    {
        Task DeleteOwnerPigeonPairForYearAsync(int year, OwnerPigeonPair ownerPigeonPair);
        Task DeleteOwnerPigeonPairForYearAsync(int year, Pigeon pigeon);
        Task<IEnumerable<OwnerPigeonPair>> GetOwnerPigeonPairsByYearAsync(int year);
        Task UpdatePigeonForOwnerAsync(int year, OwnerPigeonPair ownerPigeonPair);
    }
}
