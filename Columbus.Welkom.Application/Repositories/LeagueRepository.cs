using Columbus.Models.Owner;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories;

public class LeagueRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<LeagueEntity>(contextFactory), ILeagueRepository
{
    public async Task<ICollection<LeagueEntity>> GetAllWithOwnersAsync()
    {
        using DataContext context = _contextFactory.CreateDbContext();

        return await context.Leagues.Include(l => l.LeagueOwners)
            .ThenInclude(lo => lo.Owner)
            .ToListAsync();
    }

    public async Task<LeagueEntity?> GetByRankAsync(int rank)
    {
        using DataContext context = _contextFactory.CreateDbContext();

        return await context.Leagues.Include(l => l.LeagueOwners)
            .ThenInclude(lo => lo.Owner)
            .FirstOrDefaultAsync(l => l.Rank == rank);
    }

    public async Task<bool> UpdateWithOwnersAsync(LeagueEntity league)
    {
        using DataContext context = _contextFactory.CreateDbContext();

        LeagueEntity? existingLeague = await context.Leagues.Include(l => l.LeagueOwners)
            .FirstOrDefaultAsync(l => l.Rank == league.Rank);
        if (existingLeague is null)
            return false;

        existingLeague.Name = league.Name;

        Dictionary<OwnerId, LeagueOwnerEntity> leagueOwnersByOwnerId = league.LeagueOwners.ToDictionary(lo => lo.OwnerId);

        foreach (LeagueOwnerEntity existingLeagueOwner in existingLeague.LeagueOwners.ToList())
        {
            if (!leagueOwnersByOwnerId.Remove(existingLeagueOwner.OwnerId))
                context.LeagueOwners.Remove(existingLeagueOwner);
        }

        foreach (LeagueOwnerEntity leagueOwner in leagueOwnersByOwnerId.Values)
        {
            leagueOwner.LeagueRank = league.Rank;
            context.LeagueOwners.Add(leagueOwner);
        }

        await context.SaveChangesAsync();

        return true;
    }
}
