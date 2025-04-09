using Neptune.Database.Core.Interfaces.DataAccess;
using Neptune.Rest.Server.Entities;
using Neptune.Server.Core.Interfaces.Services;

namespace Neptune.Rest.Server.Services;

public class MessageService : IMessageService
{
    private readonly ILogger _logger;

    private readonly IDataAccess<UserEntity> _userDataAccess;


    public MessageService(ILogger<MessageService> logger, IDataAccess<UserEntity> userDataAccess)
    {
        _logger = logger;
        _userDataAccess = userDataAccess;
    }

    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }

    public Task DispatchMessageAsync(string from, string to, string message)
    {
        return Task.CompletedTask;
    }
}
