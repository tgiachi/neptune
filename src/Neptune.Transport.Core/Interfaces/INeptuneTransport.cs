using Neptune.Packets.Messages;

namespace Neptune.Transport.Core.Interfaces;

public interface INeptuneTransport
{
    string Name { get; }

    Task SendMessageAsync(NeptuneMessage message);

    Task<NeptuneMessage> OnMessageReceivedAsync(byte[] payload);
}
