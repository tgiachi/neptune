using Microsoft.Extensions.Logging;
using Neptune.Packets.Messages;
using Neptune.Transport.Core.Interfaces;

namespace Neptune.Udp.Transport;

public class UdpTransport : INeptuneTransport
{
    public string Name => "UDP";

    private readonly ILogger _logger;

    public UdpTransport(ILogger<UdpTransport> logger)
    {
        _logger = logger;
    }

    public Task SendMessageAsync(NeptuneMessage message)
    {
        throw new NotImplementedException();
    }

    public Task<NeptuneMessage> OnMessageReceivedAsync(byte[] payload)
    {
        throw new NotImplementedException();
    }
}
