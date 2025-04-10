using Microsoft.Extensions.DependencyInjection;
using Neptune.Server.Core.Interfaces;
using Neptune.Server.Core.Interfaces.Containers;
using Serilog;

namespace Neptune.Server.Core.Extensions;

public static class RegisterModuleExtension
{
    public static IServiceCollection AddNeptuneModule(this IServiceCollection services, Type moduleType)
    {
        ArgumentNullException.ThrowIfNull(moduleType);

        if (!typeof(INeptuneContainerModule).IsAssignableFrom(moduleType))
        {
            throw new ArgumentException(
                $"Type {moduleType.Name} does not implement {nameof(INeptuneContainerModule)}",
                nameof(moduleType)
            );
        }

        Log.ForContext(moduleType).Debug("Registering NeptuneModule {ModuleType}", moduleType.Name);
        var module = (INeptuneContainerModule)Activator.CreateInstance(moduleType);

        module.Initialize(services);

        return services;
    }

    public static IServiceCollection AddNeptuneModule<TModule>(this IServiceCollection services)
    {
        return AddNeptuneModule(services, typeof(TModule));
    }
}
