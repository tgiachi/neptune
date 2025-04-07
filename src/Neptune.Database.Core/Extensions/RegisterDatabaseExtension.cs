using Microsoft.Extensions.DependencyInjection;
using Neptune.Database.Core.Data;
using Neptune.Database.Core.Impl.DataAccess;
using Neptune.Database.Core.Impl.Services;
using Neptune.Database.Core.Interfaces.DataAccess;
using Neptune.Database.Core.Interfaces.Entities;
using Neptune.Database.Core.Interfaces.Services;
using Neptune.Database.Core.Types;

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
        this IServiceCollection services, string connectionString, DatabaseType databaseType, bool enableLogging = false
    )
    {
        if (!_entityTypes.Any())
        {
            throw new InvalidOperationException("No Entity Types have been registered.");
        }

        services.AddFreeRepository();

        Func<IServiceProvider, IFreeSql> fsqlFactory = r =>
        {
            var fsqlBuilder = new FreeSql.FreeSqlBuilder()
                .UseConnectionString(databaseType.ToDataType(), connectionString)
                .UseAutoSyncStructure(true);


            if (enableLogging)
            {
                fsqlBuilder.UseMonitorCommand(cmd => Console.Write(cmd.CommandText));
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
