using Columbus.Welkom.Application.Models;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface IPigeonSwapService
    {
        Task DeletePigeonSwapPairForYearAsync(int year, PigeonSwapPair pigeonSwapPair);
        Task ExportToPdf(IEnumerable<PigeonSwapPair> pigeonSwapPairs);
        Task<IEnumerable<PigeonSwapPair>> GetPigeonSwapPairsByYearAsync(int year);
        Task UpdatePigeonSwapPairAsync(int year, PigeonSwapPair pigeonSwapPair);
    }
}
