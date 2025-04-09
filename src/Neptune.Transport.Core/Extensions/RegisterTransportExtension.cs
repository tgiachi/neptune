using Microsoft.Extensions.DependencyInjection;
using Neptune.Transport.Core.Interfaces;

namespace Neptune.Transport.Core.Extensions;

public static class RegisterTransportExtension
{
    public static IServiceCollection AddNeptuneTransport<TTransport>(
        this IServiceCollection services, object? transportConfig = null
    )
        where TTransport : class, INeptuneTransport
    {
        services.AddSingleton<INeptuneTransport, TTransport>();

        if (transportConfig != null)
        {
            services.AddSingleton(transportConfig);
        }

        return services;
    }
}
