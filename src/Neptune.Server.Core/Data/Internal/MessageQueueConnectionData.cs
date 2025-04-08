using Neptune.Server.Core.Types;

namespace Neptune.Server.Core.Data.Internal;

public record MessageQueueConnectionData(MessageQueueType Type, string Hostname, int? Port, string? QueueName);
