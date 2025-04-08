using Neptune.Packets.Messages;

namespace Neptune.Database.Core.Data.Messages;

public class NeptuneQueueMessage
{
    public string Source { get; set; }

    public NeptuneMessage Message { get; set; }
}
