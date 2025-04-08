using System;

namespace Neptune.Packets.Messages;

public class NeptuneMessage
{
    public string Id { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public long Timestamp { get; set; }
    public string Payload { get; set; }
    public string Signature { get; set; }
    public int Hops { get; set; }
    public int MaxHops { get; set; }
    public string History { get; set; } // compact history string: "deviceId:lat,lon;deviceId2:lat,lon"

    public NeptuneMessage()
    {
        Id = Guid.NewGuid().ToString();
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Hops = 0;
        MaxHops = 5;
        History = "";
    }

    public void AppendToHistory(string deviceId, double latitude, double longitude)
    {
        string entry = $"{deviceId}:{latitude:F4},{longitude:F4}";
        if (string.IsNullOrWhiteSpace(History))
        {
            History = entry;
        }
        else
        {
            History += ";" + entry;
        }
    }
}
