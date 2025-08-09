using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories;

public class PigeonSaleClassRepository(DataContext context) : BaseRepository<PigeonSaleClassEntity>(context), IPigeonSaleClassRepository
{
    public async Task<ICollection<PigeonSaleClassEntity>> GetAllWithPigeonSalesAsync()
    {
        return await _context.PigeonSaleClasses
            .Include(psc => psc.PigeonSales)
            .ThenInclude(psc => psc.Seller)
            .Include(psc => psc.PigeonSales)
            .ThenInclude(psc => psc.Buyer)
            .Include(psc => psc.PigeonSales)
            .ThenInclude(psc => psc.Pigeon)
            .ToListAsync();
    }

    public async Task<PigeonSaleClassEntity?> GetByIdAsync(int id)
    {
        return await _context.PigeonSaleClasses.FirstOrDefaultAsync(psc => psc.Id == id);
    }
}
