using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, IEntity
    {
        protected readonly IDbContextFactory<DataContext> _contextFactory;

        public BaseRepository(IDbContextFactory<DataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            DataContext context = _contextFactory.CreateDbContext();

            return await context.Set<T>().ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            DataContext context = _contextFactory.CreateDbContext();

            context.Add(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            DataContext context = _contextFactory.CreateDbContext();

            context.AddRange(entities);
            await context.SaveChangesAsync();

            return entities;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            DataContext context = _contextFactory.CreateDbContext();

            context.Update(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities)
        {
            DataContext context = _contextFactory.CreateDbContext();

            context.UpdateRange(entities);
            await context.SaveChangesAsync();

            return entities;
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            DataContext context = _contextFactory.CreateDbContext();

            context.Remove(entity);
            int count = await context.SaveChangesAsync();

            return count == 1;
        }

        public virtual async Task<bool> DeleteRangeAsync(IEnumerable<T> entities)
        {
            DataContext context = _contextFactory.CreateDbContext();

            context.RemoveRange(entities);
            int count = await context.SaveChangesAsync();

            return count == entities.Count();
        }
    }
}
