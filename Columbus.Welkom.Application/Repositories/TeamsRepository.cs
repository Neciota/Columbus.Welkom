using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories;

public class TeamsRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<TeamEntity>(contextFactory), ITeamsRepository
{
    public async Task<ICollection<TeamEntity>> GetAllWithTeamOwnersAync()
    {
        DataContext context = _contextFactory.CreateDbContext();

        return await context.Teams.Include(t => t.TeamOwners)
            .ThenInclude(to => to.Owner)
            .ToListAsync();
    }

    public async Task<TeamEntity?> GetByNumberAsync(int number)
    {
        DataContext context = _contextFactory.CreateDbContext();

        return await context.Teams.Include(t => t.TeamOwners)
            .ThenInclude(to => to.Owner)
            .FirstOrDefaultAsync(t => t.Number == number);
    }
}
