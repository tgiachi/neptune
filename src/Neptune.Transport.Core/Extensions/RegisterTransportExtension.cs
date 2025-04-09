using Microsoft.Extensions.DependencyInjection;
using Neptune.Transport.Core.Interfaces;

namespace Neptune.Transport.Core.Extensions;

public static class RegisterTransportExtension
{
    public static IServiceCollection AddNeptuneTransport<TTransport>(this IServiceCollection services)
        where TTransport : class, INeptuneTransport
    {
        services.AddSingleton<INeptuneTransport, TTransport>();
        return services;
    }
}
