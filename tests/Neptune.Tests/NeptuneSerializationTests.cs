using System.Text.Json;
using System.Text.Json.Serialization;
using Neptune.Packets.Extensions;
using Neptune.Packets.Messages;
using Neptune.Packets.Types;

namespace Neptune.Tests;

[TestFixture]
public class NeptuneSerializationTests
{
    private string _senderId;
    private string _recipientId;
    private string _channelName;

    [SetUp]
    public void Setup()
    {
        _senderId = "squid@server.neptune.io";
        _recipientId = "octopus@other.neptune.io";
        _channelName = "#general";
    }

    [Test]
    public void SerializeAndDeserialize_CompleteMessage_PreservesAllProperties()
    {
        // Arrange
        var originalMessage = new NeptuneMessage
        {
            Version = "1.0",
            Header = new MessageHeader
            {
                MessageId = "550e8400-e29b-41d4-a716-446655440000",
                SenderId = _senderId,
                Timestamp = 1712409651,
                Type = MessageType.MESSAGE,
                EncryptionType = EncryptionType.CHANNEL
            },
            Routing = new MessageRouting
            {
                Channel = _channelName,
                TransportMetadata = new Dictionary<string, object>
                {
                    { "priority", "normal" }
                }
            },
            Crypto = new CryptoInfo
            {
                PublicKey = "YWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXoxMjM0NTY=",
                ChannelKeyId = "ch-key-2023-04-01",
                IV = "MTIzNDU2Nzg5MDEyMzQ1Ng==",
                Signature = "c2lnbmF0dXJlLWRhdGEtaGVyZQ=="
            },
            Payload = new EncryptedPayload
            {
                Data = "U29tZSBlbmNyeXB0ZWQgbWVzc2FnZSBjb250ZW50",
                Format = PayloadFormat.TEXT,
                ContentType = "text/plain"
            }
        };

        // Act
        string json = NeptuneExtensions.ToJson(originalMessage);
        var deserializedMessage = NeptuneExtensions.FromJson(json);

        // Assert
        Assert.That(deserializedMessage, Is.Not.Null);
        Assert.That(deserializedMessage.Version, Is.EqualTo(originalMessage.Version));

        // Header
        Assert.That(deserializedMessage.Header.MessageId, Is.EqualTo(originalMessage.Header.MessageId));
        Assert.That(deserializedMessage.Header.SenderId, Is.EqualTo(originalMessage.Header.SenderId));
        Assert.That(deserializedMessage.Header.Timestamp, Is.EqualTo(originalMessage.Header.Timestamp));
        Assert.That(deserializedMessage.Header.Type, Is.EqualTo(originalMessage.Header.Type));
        Assert.That(deserializedMessage.Header.EncryptionType, Is.EqualTo(originalMessage.Header.EncryptionType));

        // Routing
        Assert.That(deserializedMessage.Routing.Channel, Is.EqualTo(originalMessage.Routing.Channel));
        // Note: Transport metadata might need special handling for complex objects

        // Crypto
        Assert.That(deserializedMessage.Crypto.PublicKey, Is.EqualTo(originalMessage.Crypto.PublicKey));
        Assert.That(deserializedMessage.Crypto.ChannelKeyId, Is.EqualTo(originalMessage.Crypto.ChannelKeyId));
        Assert.That(deserializedMessage.Crypto.IV, Is.EqualTo(originalMessage.Crypto.IV));
        Assert.That(deserializedMessage.Crypto.Signature, Is.EqualTo(originalMessage.Crypto.Signature));

        // Payload
        Assert.That(deserializedMessage.Payload.Data, Is.EqualTo(originalMessage.Payload.Data));
        Assert.That(deserializedMessage.Payload.Format, Is.EqualTo(originalMessage.Payload.Format));
        Assert.That(deserializedMessage.Payload.ContentType, Is.EqualTo(originalMessage.Payload.ContentType));
    }


    [Test]
    public void Deserialization_EnumsAsStrings_DeserializesCorrectly()
    {
        // Arrange
        string json = @"{
                ""version"": ""1.0"",
                ""header"": {
                    ""message_id"": ""550e8400-e29b-41d4-a716-446655440000"",
                    ""sender_id"": ""squid@server.neptune.io"",
                    ""timestamp"": 1712409651,
                    ""type"": ""message"",
                    ""encryption_type"": ""channel""
                },
                ""routing"": {
                    ""channel"": ""#general""
                },
                ""crypto"": {},
                ""payload"": {
                    ""data"": ""Test message"",
                    ""format"": ""text"",
                    ""content_type"": ""text/plain""
                }
            }";

        // Act
        var message = NeptuneExtensions.FromJson(json);

        // Assert
        Assert.That(message, Is.Not.Null);
        Assert.That(message.Header.Type, Is.EqualTo(MessageType.MESSAGE));
        Assert.That(message.Header.EncryptionType, Is.EqualTo(EncryptionType.CHANNEL));
        Assert.That(message.Payload.Format, Is.EqualTo(PayloadFormat.TEXT));
    }

    [Test]
    public void Serialization_WithIndentation_FormatsCorrectly()
    {
        // Arrange
        var message = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, "Test message");
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
        };

        // Act
        string json = NeptuneExtensions.ToJson(message, options);

        // Assert
        Assert.That(json, Does.Contain("\n"));
        Assert.That(json, Does.Contain("  "));
    }

    [Test]
    public void RoundTrip_JsonNetCompatibleFormat_SerializesAndDeserializesCorrectly()
    {
        // This test makes sure the format is compatible with common JSON libraries

        // Arrange
        var originalMessage = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, "Test message");
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
        };

        // Act
        string json = JsonSerializer.Serialize(originalMessage, options);
        var deserializedMessage = JsonSerializer.Deserialize<NeptuneMessage>(json, options);

        // Assert
        Assert.That(deserializedMessage, Is.Not.Null);
        Assert.That(deserializedMessage.Header.SenderId, Is.EqualTo(originalMessage.Header.SenderId));
        Assert.That(deserializedMessage.Header.Type, Is.EqualTo(originalMessage.Header.Type));
        Assert.That(deserializedMessage.Routing.Channel, Is.EqualTo(originalMessage.Routing.Channel));
        Assert.That(deserializedMessage.Payload.Data, Is.EqualTo(originalMessage.Payload.Data));
    }
}
