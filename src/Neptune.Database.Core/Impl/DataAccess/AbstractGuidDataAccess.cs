using System.Linq.Expressions;
using FreeSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neptune.Database.Core.Interfaces.DataAccess;
using Neptune.Database.Core.Interfaces.Entities;

namespace Neptune.Database.Core.Impl.DataAccess;

public class AbstractGuidDataAccess<TEntity> : IDataAccess<TEntity>
    where TEntity : class, IDbEntity
{
    private readonly ILogger _logger;
    private readonly IBaseRepository<TEntity> _repository;

    public AbstractGuidDataAccess(ILogger<TEntity> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _repository = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IBaseRepository<TEntity>>();
    }

    public Task<long> CountAsync()
    {
        return _repository.Select.CountAsync();
    }

    public Task<List<TEntity>> FindAllAsync()
    {
        return _repository.Select.ToListAsync();
    }

    public Task<TEntity> InsertAsync(TEntity entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        return _repository.InsertAsync(entity);
    }

    public Task<List<TEntity>> InsertAsync(List<TEntity> entity)
    {
        entity.ForEach(
            e =>
            {
                e.Id = Guid.NewGuid();
                e.CreatedAt = DateTime.UtcNow;
            }
        );
        return _repository.InsertAsync(entity);
    }

    public Task<List<TEntity>> InsertBulkAsync(List<TEntity> entities)
    {
        entities.ForEach(
            e =>
            {
                e.Id = Guid.NewGuid();
                e.CreatedAt = DateTime.UtcNow;
            }
        );
        return _repository.InsertAsync(entities);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);

        return entity;
    }

    public async Task<List<TEntity>> UpdateAsync(List<TEntity> entities)
    {
        entities.ForEach(e => e.UpdatedAt = DateTime.UtcNow);
        await _repository.UpdateAsync(entities);

        return entities;
    }

    public Task<TEntity> InsertOrUpdateAsync(TEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        return _repository.InsertOrUpdateAsync(entity);
    }

    public Task<TEntity> FindByIdAsync(Guid id)
    {
        return _repository.Select.Where(e => e.Id == id).FirstAsync();
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        var result = await _repository.DeleteAsync(entity);

        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _repository.DeleteAsync(entity => entity.Id == id);

        return result > 0;
    }

    public Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func)
    {
        return _repository.Select.Where(func).ToListAsync();
    }

    public Task<TEntity?> QuerySingleAsync(Expression<Func<TEntity, bool>> func)
    {
        return _repository.Select.Where(func).FirstAsync();
    }
}
