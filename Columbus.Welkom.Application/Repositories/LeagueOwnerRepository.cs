using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories;

public class LeagueOwnerRepository(IDbContextFactory<DataContext> contextFactory) : BaseRepository<LeagueOwnerEntity>(contextFactory), ILeagueOwnerRepository
{
}
