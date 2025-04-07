using Microsoft.Extensions.DependencyInjection;
using Neptune.Transport.Core.Base;
using Neptune.Transport.Core.Interfaces;

namespace Neptune.Transport.Core.Extensions;

/// <summary>
/// Extension methods for configuring Neptune.Core.Transport
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Neptune Transport services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddNeptuneTransport(this IServiceCollection services)
    {
        services.AddSingleton<ITransportRoutingStrategy, DefaultTransportRoutingStrategy>();
        services.AddSingleton<ITransportManager, TransportManager>();

        return services;
    }

    /// <summary>
    /// Adds Neptune Transport services to the dependency injection container with a custom transport factory
    /// </summary>
    public static IServiceCollection AddNeptuneTransport<TFactory>(this IServiceCollection services)
        where TFactory : class, ITransportFactory
    {
        services.AddSingleton<ITransportRoutingStrategy, DefaultTransportRoutingStrategy>();
        services.AddSingleton<ITransportManager, TransportManager>();
        services.AddSingleton<ITransportFactory, TFactory>();

        return services;
    }
}
