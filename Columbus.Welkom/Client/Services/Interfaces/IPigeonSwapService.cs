﻿using Columbus.Welkom.Client.Models;

namespace Columbus.Welkom.Client.Services.Interfaces
{
    public interface IPigeonSwapService
    {
        Task DeletePigeonSwapPairForYearAsync(int year, PigeonSwapPair pigeonSwapPair);
        Task ExportToPdf(IEnumerable<PigeonSwapPair> pigeonSwapPairs);
        Task<IEnumerable<PigeonSwapPair>> GetPigeonSwapPairsByYearAsync(int year);
        Task UpdatePigeonSwapPairAsync(int year, PigeonSwapPair pigeonSwapPair);
    }
}
