using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Neptune.Packets.Messages;
using Neptune.Transport.Core.Extensions;
using Neptune.Transport.Core.Interfaces;
using Neptune.Udp.Transport.Config;

namespace Neptune.Udp.Transport;

public class UdpTransport : INeptuneTransport
{
    public string Name => "UDP";


    private readonly ILogger _logger;

    private readonly UdpTransportConfig _transportConfig;

    public UdpTransport(ILogger<UdpTransport> logger, UdpTransportConfig transportConfig)
    {
        _logger = logger;
        _transportConfig = transportConfig;

        _transportConfig.IpAddress ??= "255.255.255.255";
    }

    public async Task SendMessageAsync(NeptuneMessage message)
    {
        using var udpClient = new UdpClient(_transportConfig.Port);
        udpClient.EnableBroadcast = true;

        var payload = message.ToCbor();

        using var stream = new MemoryStream();

        stream.Write(Encoding.ASCII.GetBytes("NEPT"), 0, 4);

        var lengthBytes = BitConverter.GetBytes(payload.Length);

        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(lengthBytes);
        }

        stream.Write(lengthBytes, 0, 4);

        stream.Write(payload, 0, payload.Length);


        var packet = stream.ToArray();

        _logger.LogDebug("Sending UDP packet: {PacketLenght}", packet.Length);
        var byteWritten = await udpClient.SendAsync(
            packet,
            packet.Length,
            new IPEndPoint(IPAddress.Parse("255.255.255.255"), _transportConfig.Port)
        );

        _logger.LogDebug("Sent {ByteWritten} bytes to UDP", byteWritten);
    }

    public Task<NeptuneMessage> OnMessageReceivedAsync(byte[] payload)
    {
        return null;
    }
}
