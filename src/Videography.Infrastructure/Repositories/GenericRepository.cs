using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Videography.Application.Helpers;
using Videography.Application.Interfaces.Repositories;
using Videography.Infrastructure.Data;

namespace Videography.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext context;
    private readonly DbSet<T> dbSet;

    public GenericRepository(ApplicationDbContext _context)
    {
        context = _context;
        dbSet = context.Set<T>();
    }

    public virtual IQueryable<T> Entities => context.Set<T>();

    public virtual async Task CreateAsync(T entity)
    {
        await dbSet.AddAsync(entity);

    }

    public virtual async Task CreateRangeAsync(IEnumerable<T> entities)
    {
        await dbSet.AddRangeAsync(entities);
    }
    public virtual async Task<T?> FindByIdAsync(int id)
    {
        return await dbSet.FindAsync(id);
    }
    public virtual async Task<T?> FindByIdAsync(object?[] index)
    {
        return await dbSet.FindAsync(index);
    }
    public async Task<T?> FindByAsync(
       Expression<Func<T, bool>> expression,
       Func<IQueryable<T>, IQueryable<T>>? includeFunc = null)
    {
        IQueryable<T> query = dbSet;

        if (includeFunc != null)
        {
            query = includeFunc(query);
        }

        return await query.FirstOrDefaultAsync(expression);
    }

    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? includeFunc = null)
    {
        IQueryable<T> query = dbSet;

        if (expression != null)
        {
            query = query.Where(expression);
        }

        if (includeFunc != null)
        {
            query = includeFunc(query);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.ToListAsync();
    }

    public async Task<(int, PaginatedList<T>)> FindAsync(
        int pageIndex = 0,
        int pageSize = 10,
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? includeFunc = null)
    {
        IQueryable<T> query = dbSet;

        if (expression != null)
        {
            query = query.Where(expression);
        }

        if (includeFunc != null)
        {
            query = includeFunc(query);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        var paginatedList = await PaginatedList<T>.CreateAsync(query, pageIndex, pageSize);

        return (pageSize, paginatedList);
    }

    public virtual Task RemoveAsync(T entity)
    {
        dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task RemoveRangeAsync(IEnumerable<T> entities)
    {
        dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(T entity)
    {
        //dbSet.Attach(entity);
        dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByAsync(Expression<Func<T, bool>>? expression = null)
    {
        IQueryable<T> query = dbSet;

        if (expression != null)
        {
            query = query.Where(expression);
        }

        return await query.AnyAsync();
    }
}