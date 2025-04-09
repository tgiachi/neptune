using AbyssIrc.Signals.Interfaces.Listeners;
using AbyssIrc.Signals.Interfaces.Services;
using Neptune.Core.Extensions;
using Neptune.Core.Utils;
using Neptune.Database.Core.Interfaces.DataAccess;
using Neptune.Rest.Server.Entities;
using Neptune.Server.Core.Data.Config;
using Neptune.Server.Core.Data.Cryptography;
using Neptune.Server.Core.Data.Internal;
using Neptune.Server.Core.Data.Rest;
using Neptune.Server.Core.Events.Messages;
using Neptune.Server.Core.Interfaces.Services;

namespace Neptune.Rest.Server.Services;

public class MessageService : IMessageService, IAbyssSignalListener<IncomingMessageEvent>
{
    private readonly ILogger _logger;

    private readonly IDataAccess<UserEntity> _userDataAccess;

    private readonly IDataAccess<MessageEntity> _messageDataAccess;

    private readonly NeptuneServerConfig _serverConfig;

    private readonly IAbyssSignalService _abyssSignalService;


    public MessageService(
        ILogger<MessageService> logger, IDataAccess<UserEntity> userDataAccess, NeptuneServerConfig serverConfig,
        IDataAccess<MessageEntity> messageDataAccess, IAbyssSignalService abyssSignalService
    )
    {
        _logger = logger;
        _userDataAccess = userDataAccess;
        _serverConfig = serverConfig;
        _messageDataAccess = messageDataAccess;
        _abyssSignalService = abyssSignalService;

        _abyssSignalService.Subscribe(this);
    }

    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }

    public async Task<Guid> DispatchMessageAsync(string from, string to, string message)
    {
        var messageId = Guid.NewGuid();

        var outGoingMessage =
            new OutgoingMessageData(from, to, messageId.ToString(), message, _serverConfig.NodeId).ToJson();


        await _abyssSignalService.PublishAsync(
            new SendMessageQueueEvent()
            {
                MessageId = messageId.ToString(),
                Payload =
                    outGoingMessage.EncryptString(_serverConfig.SharedKey)
            }
        );


        return Guid.Empty;
    }

    public async Task OnEventAsync(IncomingMessageEvent signalEvent)
    {
        if (signalEvent.To.StartsWith('#'))
        {
            _logger.LogInformation("Dispatch to channel {Channel}", signalEvent.To);

            return;
        }

        var username = signalEvent.To.Split('@')[0];
        var nodeId = signalEvent.To.Split('@')[1];

        if (nodeId == _serverConfig.NodeName)
        {
            _logger.LogInformation("Dispatch to local user {Username}", signalEvent.To);

            await SendMessageToLocalUser(signalEvent.From, username, signalEvent.Message);

            return;
        }

        _logger.LogInformation("Dispatch to remote user {Username}", signalEvent.To);
    }

    private async Task<Guid> SendMessageToLocalUser(string from, string to, string message)
    {
        var fromUser = from.Split('@')[0];
        var toUser = to.Split('@')[0];

        var toUserEntity = await _userDataAccess.QuerySingleAsync(entity => entity.Username == fromUser);

        if (toUserEntity == null)
        {
            _logger.LogWarning("User {Username} not found", toUser);
            return Guid.Empty;
        }

        var enc = new NeptuneCryptObject();
        enc.ImportPublicKeyBase64(toUserEntity.PublicKey);

        var encryptedMessage = enc.EncryptMessage(message);

        var signature = HashUtils.ComputeSha256Hash(message);

        var messageEntity = new MessageEntity()
        {
            From = from,
            To = to,
            Payload = encryptedMessage,
            CreatedAt = DateTime.UtcNow,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Hops = 0,
            MaxHops = 5,
            Signature = signature,
            Inbox = true,
        };

        messageEntity = await _messageDataAccess.InsertAsync(messageEntity);

        _logger.LogInformation("Message sent to {Username}", toUser);

        return messageEntity.Id;
    }
}
