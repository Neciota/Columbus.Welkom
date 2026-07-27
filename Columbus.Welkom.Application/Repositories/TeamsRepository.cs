using Columbus.Models.Owner;
using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories;

public class TeamsRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<TeamEntity>(contextFactory), ITeamsRepository
{
    public async Task<ICollection<TeamEntity>> GetAllWithTeamOwnersAync()
    {
        using DataContext context = _contextFactory.CreateDbContext();

        return await context.Teams.Include(t => t.TeamOwners)
            .ThenInclude(to => to.Owner)
            .ToListAsync();
    }

    public async Task<TeamEntity?> GetByNumberAsync(int number)
    {
        using DataContext context = _contextFactory.CreateDbContext();

        return await context.Teams.Include(t => t.TeamOwners)
            .ThenInclude(to => to.Owner)
            .FirstOrDefaultAsync(t => t.Number == number);
    }

    public async Task SetTeamOwnersAsync(int teamNumber, IEnumerable<OwnerTeamEntity> teamOwners)
    {
        using DataContext context = _contextFactory.CreateDbContext();

        ICollection<OwnerTeamEntity> existingTeamOwners = await context.OwnerTeams
            .Where(to => to.TeamNumber == teamNumber)
            .ToListAsync();
        Dictionary<OwnerId, OwnerTeamEntity> teamOwnersByOwnerId = teamOwners.ToDictionary(to => to.OwnerId);

        foreach (OwnerTeamEntity existingTeamOwner in existingTeamOwners)
        {
            if (teamOwnersByOwnerId.Remove(existingTeamOwner.OwnerId, out OwnerTeamEntity? teamOwner))
                existingTeamOwner.Position = teamOwner.Position;
            else
                context.OwnerTeams.Remove(existingTeamOwner);
        }

        foreach (OwnerTeamEntity teamOwner in teamOwnersByOwnerId.Values)
        {
            teamOwner.TeamNumber = teamNumber;
            context.OwnerTeams.Add(teamOwner);
        }

        await context.SaveChangesAsync();
    }
}
