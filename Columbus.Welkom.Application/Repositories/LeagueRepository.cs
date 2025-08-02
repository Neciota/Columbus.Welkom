using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories;

public class LeagueRepository(DataContext context) : BaseRepository<LeagueEntity>(context), ILeagueRepository
{
    public async Task<ICollection<LeagueEntity>> GetAllWithOwnersAsync()
    {
        return await _context.Leagues.Include(l => l.LeagueOwners)
            .ThenInclude(lo => lo.Owner)
            .ToListAsync();
    }

    public async Task<LeagueEntity?> GetByRankAsync(int rank)
    {
        return await _context.Leagues.Include(l => l.LeagueOwners)
            .ThenInclude(lo => lo.Owner)
            .FirstOrDefaultAsync(l => l.Rank == rank);
    }
}
