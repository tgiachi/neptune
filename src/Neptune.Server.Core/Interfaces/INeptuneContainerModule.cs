using Microsoft.Extensions.DependencyInjection;

namespace Neptune.Server.Core.Interfaces;

public interface INeptuneContainerModule
{
    ServiceCollection Initialize(IServiceCollection services);
}
