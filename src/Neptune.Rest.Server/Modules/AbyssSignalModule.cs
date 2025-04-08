using AbyssIrc.Signals.Data.Configs;
using AbyssIrc.Signals.Interfaces.Services;
using AbyssIrc.Signals.Services;
using Neptune.Server.Core.Interfaces.Containers;

namespace Neptune.Rest.Server.Modules;

public class AbyssSignalModule : INeptuneContainerModule
{
    public IServiceCollection Initialize(IServiceCollection services)
    {
        return services
            .AddSingleton(
                new AbyssIrcSignalConfig()
                {
                    DispatchTasks = Environment.ProcessorCount,
                }
            )
            .AddSingleton<IAbyssSignalService, AbyssSignalService>();
    }
}
