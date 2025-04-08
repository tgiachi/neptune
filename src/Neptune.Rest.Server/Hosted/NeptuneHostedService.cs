using Neptune.Server.Core.Data.Services;
using Neptune.Server.Core.Interfaces;
using Neptune.Server.Core.Interfaces.Services;
using Neptune.Server.Core.Interfaces.Services.Base;

namespace Neptune.Rest.Server.Hosted;

public class NeptuneHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    private readonly List<ServiceToLoadData> _serviceToLoadData;

    public NeptuneHostedService(
        ILogger<NeptuneHostedService> logger, IServiceProvider serviceProvider, List<ServiceToLoadData> serviceToLoadData
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _serviceToLoadData = serviceToLoadData;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var serviceType in _serviceToLoadData)
        {
            try
            {
                var service = _serviceProvider.GetRequiredService(serviceType.ServiceType) as INeptuneLoadableService;

                await service.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during loading service: {Service}", serviceType.ServiceType);

                throw new InvalidOperationException(
                    $"Error during loading service: {serviceType.ServiceType}", ex
                );
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
}
