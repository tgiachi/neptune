using Microsoft.Extensions.DependencyInjection;
using Neptune.Core.Extensions;
using Neptune.Server.Core.Data.Directories;
using Neptune.Server.Core.Types;
using Neptune.Transport.Core.Interfaces;

namespace Neptune.Transport.Core.Extensions;

public static class RegisterTransportExtension
{
    public static IServiceCollection AddNeptuneTransport<TTransport, TTransportConfig>(
        this IServiceCollection services, DirectoriesConfig directoriesConfig
    )
        where TTransport : class, INeptuneTransport where TTransportConfig : class
    {
        services.AddSingleton<INeptuneTransport, TTransport>();


            var transportFileName = Path.Combine(
                directoriesConfig[DirectoryType.Transports],
                typeof(TTransportConfig).Name.ToSnakeCase() + ".yml"
            );

            if (!File.Exists(transportFileName))
            {
                var transportConfigInstance = Activator.CreateInstance(typeof(TTransportConfig));

                File.WriteAllText(transportFileName, transportConfigInstance.ToYaml());
            }

            var instance = File.ReadAllText(transportFileName).FromYaml<TTransportConfig>();

            services.AddSingleton<TTransportConfig>(instance);


        return services;
    }
}
