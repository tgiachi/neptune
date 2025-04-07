using Microsoft.Extensions.Logging;
using Neptune.Database.Core.Data;
using Neptune.Database.Core.Interfaces.Services;

namespace Neptune.Database.Core.Impl.Services;

public class DatabaseService : IDatabaseService
{
    private readonly List<DbEntityType> _entityTypes;
    private readonly ILogger _logger;
    private readonly IFreeSql _freeSql;

    public DatabaseService(ILogger<DatabaseService> logger, List<DbEntityType> entityTypes, IFreeSql freeSql)
    {
        _entityTypes = entityTypes;
        _freeSql = freeSql;
        _logger = logger;
    }

    public async Task StartAsync()
    {
        _logger.LogInformation("Starting migrating database");

        foreach (var entityType in _entityTypes)
        {
            var ddl = _freeSql.CodeFirst.GetComparisonDDLStatements(new[] { entityType.EntityType });
            _logger.LogDebug("DDL for table {Table} : {ddl}", entityType.EntityType, ddl);

            _freeSql.CodeFirst.SyncStructure(entityType.EntityType);
        }

        _logger.LogInformation("Database migrated and ready");
    }

    public async Task StopAsync()
    {
    }
}
