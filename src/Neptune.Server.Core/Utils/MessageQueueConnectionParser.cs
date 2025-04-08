using System.Text.RegularExpressions;
using Neptune.Server.Core.Data.Internal;
using Neptune.Server.Core.Types;

namespace Neptune.Server.Core.Utils;

public static class MessageQueueConnectionParser
{
    public static MessageQueueConnectionData Parse(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Connection string cannot be null or empty.");
        }

        if (url.StartsWith("internal://", StringComparison.OrdinalIgnoreCase))
        {
            var hostname = url.Substring("internal://".Length);
            return new MessageQueueConnectionData(
                MessageQueueType.Internal,
                hostname,
                Port: null,
                QueueName: null
            );
        }

        var regex = new Regex(@"^(?<scheme>\w+)://(?<host>[^:/]+):(?<port>\d+)(?:/(?<queue>\w+))?$");
        var match = regex.Match(url);

        if (!match.Success)
        {
            throw new ArgumentException("Invalid message queue connection string format.");
        }

        var scheme = match.Groups["scheme"].Value.ToLower();
        var type = scheme switch
        {
            "rabbitmq" => MessageQueueType.RabbitMQ,
            _          => throw new NotSupportedException($"Message queue type '{scheme}' is not supported.")
        };

        return new MessageQueueConnectionData(
            type,
            match.Groups["host"].Value,
            int.Parse(match.Groups["port"].Value),
            match.Groups["queue"].Success ? match.Groups["queue"].Value : null
        );
    }
}
