using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories;

public class PigeonSaleRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<PigeonSaleEntity>(contextFactory), IPigeonSaleRepository
{
    public async Task<ICollection<PigeonSaleEntity>> GetAllWithOwnersAndPigeonsAsync()
    {
        using DataContext context = _contextFactory.CreateDbContext();

        return await context.PigeonSales
            .Include(ps => ps.Seller)
            .Include(ps => ps.Buyer)
            .Include(ps => ps.Pigeon)
            .ToListAsync();
    }

    public async Task<PigeonSaleEntity?> GetByIdAsync(int id)
    {
        using DataContext context = _contextFactory.CreateDbContext();

        return await context.PigeonSales.SingleOrDefaultAsync(ps => ps.Id == id);
    }
}
