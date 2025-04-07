using Microsoft.Extensions.DependencyInjection;
using Neptune.Database.Core.Data;
using Neptune.Database.Core.Impl.DataAccess;
using Neptune.Database.Core.Impl.Services;
using Neptune.Database.Core.Interfaces.DataAccess;
using Neptune.Database.Core.Interfaces.Entities;
using Neptune.Database.Core.Interfaces.Services;
using Neptune.Database.Core.Types;
using Serilog;

namespace Neptune.Database.Core.Extensions;

public static class RegisterDatabaseExtension
{
    private static readonly List<DbEntityType> _entityTypes = new();

    public static IServiceCollection AddDbEntity(this IServiceCollection services, Type entityType)
    {
        _entityTypes.Add(new DbEntityType(entityType));
        return services;
    }

    public static IServiceCollection AddDbEntity<TEntity>(this IServiceCollection services) where TEntity : class, IDbEntity
    {
        _entityTypes.Add(new DbEntityType(typeof(TEntity)));
        return services;
    }

    public static IServiceCollection RegisterDatabase(
        this IServiceCollection services, DataSourceObject dataSourceObject, bool enableLogging = false,
        params Type[] entityTypes
    )
    {
        ArgumentNullException.ThrowIfNull(dataSourceObject);

        foreach (var entityType in entityTypes)
        {
            AddDbEntity(services, entityType);
        }

        if (_entityTypes.Count == 0)
        {
            throw new InvalidOperationException("No Entity Types have been registered.");
        }

        services.AddFreeRepository();

        Func<IServiceProvider, IFreeSql> fsqlFactory = r =>
        {
            var fsqlBuilder = new FreeSql.FreeSqlBuilder()
                .UseConnectionString(dataSourceObject.DatabaseType.ToDataType(), dataSourceObject.ToString())
                .UseAutoSyncStructure(true);


            if (enableLogging)
            {
                fsqlBuilder.UseMonitorCommand(cmd => Log.Logger.Debug(cmd.CommandText));
            }

            return fsqlBuilder.Build();
        };


        services.AddSingleton(_entityTypes);

        services
            .AddSingleton<IFreeSql>(fsqlFactory);

        services.AddSingleton<IDatabaseService, DatabaseService>();

        services.AddSingleton(typeof(IDataAccess<>), typeof(AbstractGuidDataAccess<>));


        return services;
    }
}
