using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Models.ViewModels;
using Columbus.Welkom.Application.Repositories;
using Columbus.Welkom.Application.Services.Interfaces;

namespace Columbus.Welkom.Application.Services;

public class PigeonSaleService(IPigeonSaleRepository pigeonSaleRepository) : IPigeonSaleService
{
    private readonly IPigeonSaleRepository _pigeonSaleRepository = pigeonSaleRepository;

    public async Task DeleteAsync(PigeonSale pigeonSale)
    {
        PigeonSaleEntity? pigeonSaleToDelete = await _pigeonSaleRepository.GetByIdAsync(pigeonSale.Id);
        if (pigeonSaleToDelete is null)
            throw new ArgumentException("No pigeon sale with the given id exists.");

        await _pigeonSaleRepository.DeleteAsync(pigeonSaleToDelete);
    }

    public async Task<IEnumerable<PigeonSale>> GetAllAsync()
    {
        ICollection<PigeonSaleEntity> pigeonSales = await _pigeonSaleRepository.GetAllWithOwnersAndPigeonsAsync();

        return pigeonSales.Select(ps => new PigeonSale
        {
            Id = ps.Id,
            Seller = ps.Seller?.ToOwner(),
            Buyer = ps.Buyer?.ToOwner(),
            Pigeon = ps.Pigeon?.ToPigeon()
        }).ToList();
    }

    public async Task UpdateAsync(PigeonSale pigeonSale)
    {
        PigeonSaleEntity? pigeonSaleToUpdate = await _pigeonSaleRepository.GetByIdAsync(pigeonSale.Id);

        if (pigeonSaleToUpdate is null)
        {
            PigeonSaleEntity pigeonSaleToAdd = new()
            {
                SellerId = pigeonSale.Seller!.Id,
                BuyerId = pigeonSale.Buyer!.Id,
                PigeonId = pigeonSale.Pigeon!.Id,
            };

            await _pigeonSaleRepository.AddAsync(pigeonSaleToAdd);
        }
        else
        {
            pigeonSaleToUpdate.SellerId = pigeonSale.Seller!.Id;
            pigeonSaleToUpdate.BuyerId = pigeonSale.Buyer!.Id;
            pigeonSaleToUpdate.PigeonId = pigeonSale.Pigeon!.Id;

            await _pigeonSaleRepository.UpdateAsync(pigeonSaleToUpdate);
        }
    }
}
