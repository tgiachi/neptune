using System.Linq.Expressions;
using Neptune.Database.Core.Interfaces.Entities;

namespace Neptune.Database.Core.Interfaces.DataAccess;

public interface IDataAccess<TEntity> where TEntity : class, IDbEntity
{
    Task<long> CountAsync();

    Task<List<TEntity>> FindAllAsync();

    Task<TEntity> InsertAsync(TEntity entity);

    Task<List<TEntity>> InsertAsync(List<TEntity> entity);

    Task<List<TEntity>> InsertBulkAsync(List<TEntity> entities);

    Task<TEntity> UpdateAsync(TEntity entity);
    Task<List<TEntity>> UpdateAsync(List<TEntity> entities);

    Task<TEntity> InsertOrUpdateAsync(TEntity entity);

    Task<TEntity> FindByIdAsync(Guid id);

    Task<bool> DeleteAsync(TEntity entity);

    Task<bool> DeleteAsync(Guid id);

    Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func);

    Task<TEntity?> QuerySingleAsync(Expression<Func<TEntity, bool>> func);
}
