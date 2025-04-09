using System.Text;
using AbyssIrc.Signals.Interfaces.Listeners;
using AbyssIrc.Signals.Interfaces.Services;
using Neptune.Core.Extensions;
using Neptune.Database.Core.Data.Messages;
using Neptune.Packets.Messages;
using Neptune.Server.Core.Data.Config;
using Neptune.Server.Core.Data.Internal;
using Neptune.Server.Core.Events.Messages;
using Neptune.Server.Core.Extensions;
using Neptune.Server.Core.Interfaces.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Neptune.Rest.Server.Services;

public class RabbitMqMessageQueueService : IMessageQueueService, IDisposable, IAbyssSignalListener<SendMessageQueueEvent>
{
    private readonly NeptuneServerConfig _neptuneServerConfig;

    private readonly MessageQueueConnectionData _messageQueueConnection;

    private readonly IAbyssSignalService _abyssSignalService;

    private readonly ILogger _logger;

    private readonly IConnectionFactory _connectionFactory;
    private IChannel _channel;
    private IConnection _connection;

    public RabbitMqMessageQueueService(
        ILogger<RabbitMqMessageQueueService> logger, NeptuneServerConfig neptuneServerConfig,
        IAbyssSignalService abyssSignalService
    )
    {
        _neptuneServerConfig = neptuneServerConfig;
        _abyssSignalService = abyssSignalService;
        _messageQueueConnection = _neptuneServerConfig.MessagesQueue.ParseMessageQueueConnection();
        _logger = logger;
        _connectionFactory = new ConnectionFactory
        {
            HostName = _messageQueueConnection.Hostname,
            Port = _messageQueueConnection.Port ?? 5672
        };

        _abyssSignalService.Subscribe(this);
    }

    public async Task StartAsync()
    {
        await CheckAndCreateQueueAsync();
        await StartListenQueueAsync();
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }

    public async Task PublishAsync(NeptuneMessage message)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("Channel is not initialized.");
        }

        var queueMessage = new NeptuneQueueMessage
        {
            Source = _neptuneServerConfig.NodeId,
            Message = message
        };

        var body = Encoding.UTF8.GetBytes(queueMessage.ToJson());

        await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: _messageQueueConnection.QueueName, body: body);
        _logger.LogTrace("(RAW): RabbitMQ publish message: {Message}", queueMessage.ToJson());
    }


    private async Task CheckAndCreateQueueAsync()
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            _messageQueueConnection.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        );
    }

    private async Task StartListenQueueAsync()
    {
        _connection = await _connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());

            _logger.LogTrace("(RAW): RabbitMQ received message: {Message}", message);

            try
            {
                await ParseAndDispatchMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ received exception during parsing message: {Message}", message);
            }
        };

        await _channel.BasicConsumeAsync(
            _messageQueueConnection.QueueName,
            autoAck: true,
            consumer: consumer
        );


        _logger.LogInformation("RabbitMQ listening: {QueueName}", _neptuneServerConfig.MessagesQueue.ConnectionString);
    }


    private async Task ParseAndDispatchMessageAsync(string message)
    {
        try
        {
            var queueMessage = message.FromJson<SendMessageQueueEvent>();

            var messageData = queueMessage.Payload.DecryptString(_neptuneServerConfig.SharedKey)
                .FromJson<OutgoingMessageData>();


            var messageEvent = new IncomingMessageEvent(
                messageData.From,
                messageData.To,
                messageData.MessageId,
                messageData.Message,
                messageData.SourceNodeId

            );

            await _abyssSignalService.PublishAsync(messageEvent);



        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ received exception during parsing message: {Message}", message);
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task OnEventAsync(SendMessageQueueEvent signalEvent)
    {
        _logger.LogDebug("Sending message to queue: {MessageId}", signalEvent.MessageId);
        await _channel.BasicPublishAsync(
            string.Empty,
            _messageQueueConnection.QueueName,
            Encoding.UTF8.GetBytes(signalEvent.ToJson())
        );
    }
}
