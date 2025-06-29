using Columbus.Welkom.Application.Models.Entities;

namespace Columbus.Welkom.Application.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : IEntity
    {
        abstract Task<T> AddAsync(T entity);
        abstract Task<T> UpdateAsync(T entity);
        abstract Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        abstract Task<T?> GetByIdAsync(int id);
        abstract Task<IEnumerable<T>> GetAllByIdsAsync(IEnumerable<int> ids);
        abstract Task<IEnumerable<T>> GetAllAsync();
        abstract Task<bool> DeleteRangeAsync(IEnumerable<T> entities);
        abstract Task<bool> DeleteRangeAsync(IEnumerable<int> ids);
        abstract Task<bool> DeleteAsync(T entity);
        abstract Task<bool> DeleteAsync(int id);
    }
}