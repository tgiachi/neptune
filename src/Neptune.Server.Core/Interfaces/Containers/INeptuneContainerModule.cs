using Microsoft.Extensions.DependencyInjection;

namespace Neptune.Server.Core.Interfaces.Containers;

public interface INeptuneContainerModule
{
    IServiceCollection Initialize(IServiceCollection services);
}
