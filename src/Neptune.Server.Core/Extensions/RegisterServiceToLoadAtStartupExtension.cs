using Microsoft.Extensions.DependencyInjection;
using Neptune.Server.Core.Data.Services;
using Neptune.Server.Core.Interfaces;
using Neptune.Server.Core.Interfaces.Services;
using Neptune.Server.Core.Interfaces.Services.Base;

namespace Neptune.Server.Core.Extensions;

public static class RegisterServiceToLoadAtStartupExtension
{
    public static IServiceCollection RegisterServiceToLoadAtStartup(this IServiceCollection services, Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        if (!typeof(INeptuneLoadableService).IsAssignableFrom(serviceType))
        {
            throw new ArgumentException(
                $"Type {serviceType.Name} does not implement {nameof(INeptuneLoadableService)}",
                nameof(serviceType)
            );
        }

        return services.AddToRegisterTypedList(new ServiceToLoadData(serviceType));
    }

    public static IServiceCollection RegisterServiceToLoadAtStartup<TService>(this IServiceCollection services)
        where TService : INeptuneLoadableService
    {
        return RegisterServiceToLoadAtStartup(services, typeof(TService));
    }
}
