using Neptune.Server.Core.Data.Config.Sections;
using Neptune.Server.Core.Data.Internal;
using Neptune.Server.Core.Utils;

namespace Neptune.Server.Core.Extensions;

public static class MessageQueueConnectionStringParser
{
    public static MessageQueueConnectionData ParseMessageQueueConnection(this MessageQueueConfig config)
    {
        return MessageQueueConnectionParser.Parse(config.ConnectionString);
    }
}
