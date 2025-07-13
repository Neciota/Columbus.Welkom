using Columbus.Welkom.Application.Database;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Columbus.Welkom.Application.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, IEntity
    {
        protected readonly DataContext _context;

        public BaseRepository(DataContext context)
        {
            _context = context;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            _context.AddRange(entities);
            await _context.SaveChangesAsync();

            return entities;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            _context.Remove(entity);
            int count = await _context.SaveChangesAsync();

            return count == 1;
        }

        public virtual async Task<bool> DeleteRangeAsync(IEnumerable<T> entities)
        {
            _context.RemoveRange(entities);
            int count = await _context.SaveChangesAsync();

            return count == entities.Count();
        }
    }
}
