namespace Neptune.Transport.Core.Models;

/// <summary>
/// Metrics for a transport
/// </summary>
public class TransportMetrics
{
    /// <summary>
    /// Gets the number of messages sent
    /// </summary>
    public long MessagesSent { get; private set; }

    /// <summary>
    /// Gets the number of messages received
    /// </summary>
    public long MessagesReceived { get; private set; }

    /// <summary>
    /// Gets the number of bytes sent
    /// </summary>
    public long BytesSent { get; private set; }

    /// <summary>
    /// Gets the number of bytes received
    /// </summary>
    public long BytesReceived { get; private set; }

    /// <summary>
    /// Gets the number of errors
    /// </summary>
    public long Errors { get; private set; }

    /// <summary>
    /// Gets the time the transport was started
    /// </summary>
    public DateTime? StartTime { get; private set; }

    /// <summary>
    /// Gets the average round-trip time
    /// </summary>
    public double AverageRoundTripTimeMs { get; private set; }

    /// <summary>
    /// Increments the messages sent counter
    /// </summary>
    public void IncrementMessagesSent() => MessagesSent++;

    /// <summary>
    /// Increments the messages received counter
    /// </summary>
    public void IncrementMessagesReceived() => MessagesReceived++;

    /// <summary>
    /// Adds to the bytes sent counter
    /// </summary>
    public void AddBytesSent(long bytes) => BytesSent += bytes;

    /// <summary>
    /// Adds to the bytes received counter
    /// </summary>
    public void AddBytesReceived(long bytes) => BytesReceived += bytes;

    /// <summary>
    /// Increments the errors counter
    /// </summary>
    public void IncrementErrors() => Errors++;

    /// <summary>
    /// Sets the start time
    /// </summary>
    public void SetStartTime() => StartTime = DateTime.UtcNow;

    /// <summary>
    /// Updates the average round-trip time
    /// </summary>
    public void UpdateRoundTripTime(double rttMs)
    {
        if (AverageRoundTripTimeMs <= 0)
            AverageRoundTripTimeMs = rttMs;
        else
            AverageRoundTripTimeMs = (AverageRoundTripTimeMs * 0.9) + (rttMs * 0.1); // Weighted average
    }

    /// <summary>
    /// Resets all metrics
    /// </summary>
    public void Reset()
    {
        MessagesSent = 0;
        MessagesReceived = 0;
        BytesSent = 0;
        BytesReceived = 0;
        Errors = 0;
        StartTime = null;
        AverageRoundTripTimeMs = 0;
    }
}
