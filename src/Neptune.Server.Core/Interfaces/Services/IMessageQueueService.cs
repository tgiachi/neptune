using Neptune.Packets.Messages;
using Neptune.Server.Core.Interfaces.Services.Base;

namespace Neptune.Server.Core.Interfaces.Services;

public interface IMessageQueueService : INeptuneLoadableService
{
    Task PublishAsync(NeptuneMessage message);
}
