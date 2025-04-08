using Neptune.Server.Core.Data.Config.Sections;
using Neptune.Server.Core.Types;

namespace Neptune.Server.Core.Data.Config;

public class NeptuneServerConfig
{
    public DatabaseSection Database { get; set; } = new();

    public JwtAuthConfig JwtAuth { get; set; } = new();

    public MessageQueueConfig MessagesQueue { get; set; } = new();

    public DevelopmentConfig Development { get; set; } = new();

    public WebServerConfig WebServer { get; set; } = new();

    public LogLevelType LogLevel { get; set; } = LogLevelType.Information;
}
