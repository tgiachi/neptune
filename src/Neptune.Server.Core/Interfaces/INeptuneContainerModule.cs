using Microsoft.Extensions.DependencyInjection;

namespace Neptune.Server.Core.Interfaces;

public interface INeptuneContainerModule
{
    IServiceCollection Initialize(IServiceCollection services);
}
