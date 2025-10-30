using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PlanejamentoIntegrado.Data;

namespace PlanejamentoIntegrado.Repositories;

public class Repository<T>(IntegratedPlanningDbContext context) : IRepository<T>
    where T : class
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<int> SaveChanges(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> Count(Expression<Func<T, bool>> predicate, params string[] includes)
    {
        var query = ApplyIncludes(_dbSet.AsQueryable(), includes);

        return await query.CountAsync(predicate);
    }

    public async Task<decimal> Sum(
        Expression<Func<T, decimal>> sumExpression,
        Expression<Func<T, bool>>? filter = null
    )
    {
        var query = _dbSet.AsQueryable();
        if (filter != null)
            query = query.Where(filter);

        return await query.SumAsync(sumExpression);
    }

    public async Task<T?> Get(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params string[] includes
    )
    {
        var query = ApplyIncludes(_dbSet.AsQueryable(), includes);
        if (filter != null)
            query = query.Where(filter);
        if (orderBy != null)
            query = orderBy(query);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<T?> Get(int id, params string[] includes)
    {
        var query = ApplyIncludes(_dbSet.AsQueryable(), includes);

        var parameter = Expression.Parameter(typeof(T), "x");
        var idProp = Expression.Property(parameter, "Id");
        var equals = Expression.Equal(idProp, Expression.Constant(id));
        var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

        return await query.FirstOrDefaultAsync(lambda);
    }

    public async Task<List<T>> GetAll(
        Expression<Func<T, bool>>? filter = null,
        int pageNumber = 0,
        int pageSize = 0,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params string[] includes
    )
    {
        var query = ApplyIncludes(_dbSet.AsQueryable(), includes);

        if (filter != null)
            query = query.Where(filter);
        if (orderBy != null)
            query = orderBy(query);
        if (pageNumber > 0 && pageSize > 0)
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return await query.ToListAsync();
    }

    public async Task<List<T>> GetAllDistinctBy<TKey>(
        Expression<Func<T, TKey>> distinctBy,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
    )
    {
        var query = _dbSet.AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        var distinctQuery = query.GroupBy(distinctBy).Select(g => g.First());

        if (orderBy != null)
            distinctQuery = orderBy(distinctQuery);

        return await distinctQuery.ToListAsync();
    }

    public async Task<List<TResult>> GetAllDistinctBy<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<TResult>, IOrderedQueryable<TResult>>? orderBy = null
    )
    {
        var query = _dbSet.AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        var distinctQuery = query.Select(selector).Distinct();

        if (orderBy != null)
            distinctQuery = orderBy(distinctQuery);

        return await distinctQuery.ToListAsync();
    }

    public async Task<object> GetMax(
        Expression<Func<T, object>> max,
        Expression<Func<T, bool>>? filter = null,
        params string[] includes
    )
    {
        var query = ApplyIncludes(_dbSet.AsQueryable(), includes);
        if (filter != null)
            query = query.Where(filter);

        return await query.MaxAsync(max);
    }

    public Task Update(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public async Task Insert(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task Delete(int id)
    {
        var entity = await Get(id);
        if (entity != null)
            _dbSet.Remove(entity);
    }

    public async Task Delete(Expression<Func<T, bool>> filter)
    {
        var entities = await _dbSet.Where(filter).ToListAsync();
        _dbSet.RemoveRange(entities);
    }

    public Task Delete(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    private IQueryable<T> ApplyIncludes(IQueryable<T> query, params string[] includes)
    {
        if (includes == null || includes.Length == 0)
            return query;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }
}
