using Neptune.Database.Core.Interfaces.DataAccess;
using Neptune.Rest.Server.Entities;
using Neptune.Server.Core.Data.Config;
using Neptune.Server.Core.Data.Cryptography;
using Neptune.Server.Core.Interfaces.Services;

namespace Neptune.Rest.Server.Services;

public class MessageService : IMessageService
{
    private readonly ILogger _logger;

    private readonly IDataAccess<UserEntity> _userDataAccess;

    private readonly NeptuneServerConfig _serverConfig;


    public MessageService(
        ILogger<MessageService> logger, IDataAccess<UserEntity> userDataAccess, NeptuneServerConfig serverConfig
    )
    {
        _logger = logger;
        _userDataAccess = userDataAccess;
        _serverConfig = serverConfig;
    }

    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DispatchMessageAsync(string from, string to, string message)
    {
        if (to.StartsWith('#'))
        {
            _logger.LogInformation("Dispatch to channel {Channel}", to);

            return;
        }

        var username = to.Split('@')[0];
        var nodeId = to.Split('@')[1];

        if (nodeId == _serverConfig.NodeId)
        {
            _logger.LogInformation("Dispatch to local user {Username}", to);

            await SendMessageToLocalUser(from, username, message);
        }

        else
        {
            _logger.LogInformation("Dispatch to remote user {Username}", to);
        }
    }

    private async Task SendMessageToLocalUser(string from, string to, string message)
    {
        var fromUser = from.Split('@')[0];
        var toUser = to.Split('@')[0];

        var toUserEntity = await _userDataAccess.QuerySingleAsync(entity => entity.Username == fromUser);

        if (toUserEntity == null)
        {
            _logger.LogWarning("User {Username} not found", toUser);
            return;
        }
    }

}
