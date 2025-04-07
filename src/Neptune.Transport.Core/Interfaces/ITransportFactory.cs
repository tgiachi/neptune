using Neptune.Transport.Core.Models;

namespace Neptune.Transport.Core.Interfaces;

/// <summary>
/// Represents a factory for creating transport instances
/// </summary>
public interface ITransportFactory
{
    /// <summary>
    /// Creates a transport instance with the specified options
    /// </summary>
    ITransport CreateTransport(string transportType, TransportOptions options);

    /// <summary>
    /// Gets available transport types
    /// </summary>
    IEnumerable<string> GetAvailableTransportTypes();
}
