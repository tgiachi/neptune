using Neptune.Server.Core.Types;
using Serilog.Events;

namespace Neptune.Server.Core.Extensions;

public static class SerilogLogLevelExtension
{
    public static LogEventLevel ToLogEventLevel(this LogLevelType type) =>
        type switch
        {
            LogLevelType.Trace       => LogEventLevel.Verbose,
            LogLevelType.Debug       => LogEventLevel.Debug,
            LogLevelType.Information => LogEventLevel.Information,
            LogLevelType.Warning     => LogEventLevel.Warning,
            LogLevelType.Error       => LogEventLevel.Error,
            _                        => LogEventLevel.Information
        };
}
