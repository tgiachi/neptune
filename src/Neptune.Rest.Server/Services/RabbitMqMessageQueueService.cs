using System.Text;
using Neptune.Server.Core.Data.Config;
using Neptune.Server.Core.Data.Internal;
using Neptune.Server.Core.Extensions;
using Neptune.Server.Core.Interfaces.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Neptune.Rest.Server.Services;

public class RabbitMqMessageQueueService : IMessageQueueService, IDisposable
{
    private readonly NeptuneServerConfig _neptuneServerConfig;

    private readonly MessageQueueConnectionData _messageQueueConnection;

    private readonly ILogger _logger;

    private readonly IConnectionFactory _connectionFactory;
    private IChannel _channel;
    private IConnection _connection;

    public RabbitMqMessageQueueService(ILogger<RabbitMqMessageQueueService> logger, NeptuneServerConfig neptuneServerConfig)
    {
        _neptuneServerConfig = neptuneServerConfig;
        _messageQueueConnection = _neptuneServerConfig.MessagesQueue.ParseMessageQueueConnection();
        _logger = logger;
        _connectionFactory = new ConnectionFactory
        {
            HostName = _messageQueueConnection.Hostname,
            Port = _messageQueueConnection.Port ?? 5672
        };
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
        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogDebug("RabbitMQ received message: {Message}", message);
            return Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(
            _messageQueueConnection.QueueName,
            autoAck: true,
            consumer: consumer
        );


        _logger.LogInformation("RabbitMQ listening: {QueueName}", _neptuneServerConfig.MessagesQueue.ConnectionString);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}
