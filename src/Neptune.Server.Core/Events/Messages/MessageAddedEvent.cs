using Neptune.Packets.Messages;

namespace Neptune.Server.Core.Events.Messages;

public record MessageAddedEvent(NeptuneMessage Message);
