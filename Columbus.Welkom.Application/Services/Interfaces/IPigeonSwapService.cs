using Columbus.Welkom.Application.Models.ViewModels;

namespace Columbus.Welkom.Application.Services.Interfaces
{
    public interface IPigeonSwapService
    {
        Task DeletePigeonSwapPairAsync(PigeonSwapPair pigeonSwapPair);
        Task ExportToPdf(IEnumerable<PigeonSwapPair> pigeonSwapPairs);
        Task<IEnumerable<PigeonSwapPair>> GetPigeonSwapPairsAsync();
        Task UpdatePigeonSwapPairAsync(int year, PigeonSwapPair pigeonSwapPair);
    }
}
