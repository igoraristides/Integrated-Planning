using System.Linq.Expressions;

namespace PlanejamentoIntegrado.Repositories;

public interface IRepository<T>
    where T : class
{
    Task<int> SaveChanges(CancellationToken cancellationToken = default);

    Task<int> Count(Expression<Func<T, bool>> predicate, params string[] includes);

    Task<decimal> Sum(
        Expression<Func<T, decimal>> sumExpression,
        Expression<Func<T, bool>>? filter = null
    );
    Task<T?> Get(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params string[] includes
    );

    Task<T?> Get(int id, params string[] includes);

    Task<List<T>> GetAll(
        Expression<Func<T, bool>>? filter = null,
        int pageNumber = 0,
        int pageSize = 0,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params string[] includes
    );

    Task<List<T>> GetAllDistinctBy<TKey>(
        Expression<Func<T, TKey>> distinctBy,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
    );

    Task<List<TResult>> GetAllDistinctBy<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<TResult>, IOrderedQueryable<TResult>>? orderBy = null
    );

    Task<object> GetMax(
        Expression<Func<T, object>> max,
        Expression<Func<T, bool>>? filter = null,
        params string[] includes
    );

    Task Update(T entity);
    Task Insert(T entity);
    Task Delete(int id);
    Task Delete(Expression<Func<T, bool>> filter);
    Task Delete(T entity);
}
