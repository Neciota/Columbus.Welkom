using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories;

public class PigeonSaleRepository(DataContext context) : BaseRepository<PigeonSaleEntity>(context), IPigeonSaleRepository
{
    public async Task<ICollection<PigeonSaleEntity>> GetAllWithOwnersAndPigeonsAsync()
    {
        return await _context.PigeonSales
            .Include(ps => ps.Seller)
            .Include(ps => ps.Buyer)
            .Include(ps => ps.Pigeon)
            .ToListAsync();
    }

    public async Task<PigeonSaleEntity?> GetByIdAsync(int id)
    {
        return await _context.PigeonSales.SingleOrDefaultAsync(ps => ps.Id == id);
    }
}
