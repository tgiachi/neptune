namespace Neptune.Udp.Transport.Config;

public class UdpTransportConfig
{
    public string IpAddress { get; set; }

    public int Port { get; set; } = 32002;

    public bool Broadcast { get; set; } = true;
}
