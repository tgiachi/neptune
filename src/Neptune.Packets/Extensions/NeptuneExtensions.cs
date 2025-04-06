using System;
using System.Text.Json;
using Neptune.Core.Utils;
using Neptune.Packets.Messages;
using Neptune.Packets.Types;

namespace Neptune.Packets.Extensions;

/// <summary>
/// Extension methods for Neptune protocol
/// </summary>
public static class NeptuneExtensions
{
    /// <summary>
    /// Converts a Neptune message to JSON
    /// </summary>
    /// <param name="message">The message to serialize</param>
    /// <param name="options">Optional JSON serialization options</param>
    /// <returns>JSON string representation of the message</returns>
    public static string ToJson(this NeptuneMessage message, JsonSerializerOptions? options = null)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        return JsonSerializer.Serialize(message, options ?? JsonUtils.GetDefaultJsonSettings());
    }

    /// <summary>
    /// Creates a Neptune message from JSON
    /// </summary>
    /// <param name="json">JSON string representation of a Neptune message</param>
    /// <param name="options">Optional JSON deserialization options</param>
    /// <returns>Deserialized Neptune message</returns>
    public static NeptuneMessage? FromJson(string json, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentNullException(nameof(json));
        }

        return JsonSerializer.Deserialize<NeptuneMessage>(json, options ?? JsonUtils.GetDefaultJsonSettings());
    }

    /// <summary>
    /// Validates a Neptune message for basic correctness
    /// </summary>
    /// <param name="message">The message to validate</param>
    /// <returns>True if the message is valid, false otherwise</returns>
    public static bool IsValid(this NeptuneMessage message)
    {
        if (message == null)
        {
            return false;
        }

        // Check version
        if (string.IsNullOrEmpty(message.Version))
        {
            return false;
        }

        // Check header
        if (message.Header == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(message.Header.MessageId))
        {
            return false;
        }

        if (string.IsNullOrEmpty(message.Header.SenderId))
        {
            return false;
        }

        // Check routing based on message type
        if (message.Header.Type == MessageType.MESSAGE ||
            message.Header.Type == MessageType.JOIN ||
            message.Header.Type == MessageType.LEAVE)
        {
            if (string.IsNullOrEmpty(message.Routing?.Channel))
            {
                return false;
            }
        }
        else if (message.Header.Type == MessageType.PRIVMSG ||
                 message.Header.Type == MessageType.ERROR)
        {
            if (string.IsNullOrEmpty(message.Routing?.Recipient))
            {
                return false;
            }
        }

        // Check crypto info based on encryption type
        if (message.Header.EncryptionType == EncryptionType.E2E)
        {
            if (string.IsNullOrEmpty(message.Crypto?.PublicKey))
            {
                return false;
            }
        }
        else if (message.Header.EncryptionType == EncryptionType.CHANNEL)
        {
            if (string.IsNullOrEmpty(message.Crypto?.ChannelKeyId))
            {
                return false;
            }
        }

        // All validations passed
        return true;
    }

    /// <summary>
    /// Creates a deep clone of a Neptune message
    /// </summary>
    /// <param name="message">The message to clone</param>
    /// <returns>A deep copy of the message</returns>
    public static NeptuneMessage Clone(this NeptuneMessage message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Simple but effective deep clone using serialization
        string json = message.ToJson();
        return FromJson(json) ?? throw new InvalidOperationException("Failed to clone message");
    }

    /// <summary>
    /// Formats a Neptune ID into canonical form
    /// </summary>
    /// <param name="localId">Local identifier</param>
    /// <param name="server">Server domain</param>
    /// <returns>Formatted ID in the form localID@server.domain</returns>
    public static string FormatNeptuneId(string localId, string server)
    {
        if (string.IsNullOrEmpty(localId))
        {
            throw new ArgumentNullException(nameof(localId));
        }

        if (string.IsNullOrEmpty(server))
        {
            throw new ArgumentNullException(nameof(server));
        }

        return $"{localId}@{server}";
    }

    /// <summary>
    /// Attempts to parse a Neptune ID into its components
    /// </summary>
    /// <param name="id">Neptune ID in the form localID@server.domain</param>
    /// <param name="localId">Output parameter for the local ID component</param>
    /// <param name="server">Output parameter for the server component</param>
    /// <returns>True if parsing was successful, false otherwise</returns>
    public static bool TryParseNeptuneId(string id, out string localId, out string server)
    {
        localId = string.Empty;
        server = string.Empty;

        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        int atIndex = id.IndexOf('@');
        if (atIndex <= 0 || atIndex == id.Length - 1)
        {
            return false;
        }

        localId = id.Substring(0, atIndex);
        server = id.Substring(atIndex + 1);
        return true;
    }

    /// <summary>
    /// Checks if a channel name is valid according to the protocol
    /// </summary>
    /// <param name="channelName">Channel name to validate</param>
    /// <returns>True if the channel name is valid, false otherwise</returns>
    public static bool IsValidChannelName(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
        {
            return false;
        }

        // Channel must start with # or ##
        if (!channelName.StartsWith("#"))
        {
            return false;
        }

        // Channel names should be 2-32 characters
        if (channelName.Length < 2 || channelName.Length > 32)
        {
            return false;
        }

        if (channelName[0] == '#')
        {
            // Local channel names must start with a single #
            if (channelName[1] == '#')
            {
                return true; // Invalid local channel name
            }
        }
        else if (channelName[0] == '#')
        {
            // Global channel names must start with ##
            if (channelName[1] != '#')
            {
                return false; // Invalid global channel name
            }
        }

        // Only allow alphanumeric, hyphen, and underscore after the # prefix
        for (int i = 1; i < channelName.Length; i++)
        {
            char c = channelName[i];
            if (!(char.IsLetterOrDigit(c) || c == '-' || c == '_'))
            {
                return false;
            }
        }

        return true;
    }
}
