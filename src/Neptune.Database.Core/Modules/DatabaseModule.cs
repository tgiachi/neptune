using Microsoft.Extensions.DependencyInjection;
using Neptune.Server.Core.Interfaces;
using Neptune.Server.Core.Interfaces.Containers;

namespace Neptune.Database.Core.Modules;

public class DatabaseModule : INeptuneContainerModule
{
    public IServiceCollection Initialize(IServiceCollection services)
    {
        return services;
    }
}
