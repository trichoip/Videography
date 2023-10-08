using System.Linq.Expressions;

namespace Videography.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }
        Task<T?> FindByIdAsync(int id);
        Task<T?> FindByIdAsync(object?[] index);

        Task<bool> ExistsByAsync(Expression<Func<T, bool>>? expression = null);

        Task<T?> FindByAsync(
            Expression<Func<T, bool>> expression,
            Func<IQueryable<T>, IQueryable<T>>? includeFunc = null);

        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includeFunc = null);

        Task<IQueryable<T>> FindToIQueryableAsync(
            Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? includeFunc = null);

        Task UpdateAsync(T entity);
        Task CreateAsync(T entity);
        Task CreateRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
    }
}
