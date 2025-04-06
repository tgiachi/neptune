using System.Text.Json;
using Neptune.Packets.Extensions;
using Neptune.Packets.Messages;
using Neptune.Packets.Types;

namespace Neptune.Tests;

[TestFixture]
public class NeptuneMessageTests
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
    public void CreateChannelMessage_WithValidParameters_CreatesCorrectMessage()
    {
        // Arrange
        string messageText = "Hello, Neptune!";

        // Act
        var message = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, messageText);

        // Assert
        Assert.That(message, Is.Not.Null);
        Assert.That(message.Header.SenderId, Is.EqualTo(_senderId));
        Assert.That(message.Header.Type, Is.EqualTo(MessageType.MESSAGE));
        Assert.That(message.Header.EncryptionType, Is.EqualTo(EncryptionType.CHANNEL));
        Assert.That(message.Routing.Channel, Is.EqualTo(_channelName));
        Assert.That(message.Payload.Data, Is.EqualTo(messageText));
        Assert.That(message.Payload.Format, Is.EqualTo(PayloadFormat.TEXT));
    }

    [Test]
    public void CreatePrivateMessage_WithValidParameters_CreatesCorrectMessage()
    {
        // Arrange
        string messageText = "Hello, octopus!";

        // Act
        var message = NeptuneMessage.CreatePrivateMessage(_senderId, _recipientId, messageText);

        // Assert
        Assert.That(message, Is.Not.Null);
        Assert.That(message.Header.SenderId, Is.EqualTo(_senderId));
        Assert.That(message.Header.Type, Is.EqualTo(MessageType.PRIVMSG));
        Assert.That(message.Header.EncryptionType, Is.EqualTo(EncryptionType.E2E));
        Assert.That(message.Routing.Recipient, Is.EqualTo(_recipientId));
        Assert.That(message.Payload.Data, Is.EqualTo(messageText));
        Assert.That(message.Payload.Format, Is.EqualTo(PayloadFormat.TEXT));
    }

    [Test]
    public void CreateJoinMessage_WithValidParameters_CreatesCorrectMessage()
    {
        // Act
        var message = NeptuneMessage.CreateJoinMessage(_senderId, _channelName);

        // Assert
        Assert.That(message, Is.Not.Null);
        Assert.That(message.Header.SenderId, Is.EqualTo(_senderId));
        Assert.That(message.Header.Type, Is.EqualTo(MessageType.JOIN));
        Assert.That(message.Header.EncryptionType, Is.EqualTo(EncryptionType.NONE));
        Assert.That(message.Routing.Channel, Is.EqualTo(_channelName));
        Assert.That(message.Payload.Data, Is.Empty);
    }

    [Test]
    public void CreateLeaveMessage_WithValidParameters_CreatesCorrectMessage()
    {
        // Act
        var message = NeptuneMessage.CreateLeaveMessage(_senderId, _channelName);

        // Assert
        Assert.That(message, Is.Not.Null);
        Assert.That(message.Header.SenderId, Is.EqualTo(_senderId));
        Assert.That(message.Header.Type, Is.EqualTo(MessageType.LEAVE));
        Assert.That(message.Header.EncryptionType, Is.EqualTo(EncryptionType.NONE));
        Assert.That(message.Routing.Channel, Is.EqualTo(_channelName));
        Assert.That(message.Payload.Data, Is.Empty);
    }

    [Test]
    public void CreatePingMessage_WithValidSenderId_CreatesCorrectMessage()
    {
        // Act
        var message = NeptuneMessage.CreatePingMessage(_senderId);

        // Assert
        Assert.That(message, Is.Not.Null);
        Assert.That(message.Header.SenderId, Is.EqualTo(_senderId));
        Assert.That(message.Header.Type, Is.EqualTo(MessageType.PING));
        Assert.That(message.Header.EncryptionType, Is.EqualTo(EncryptionType.NONE));
        Assert.That(message.Payload.Data, Is.Not.Empty);
    }

    [Test]
    public void CreatePongMessage_WithValidPingMessage_CreatesCorrectMessage()
    {
        // Arrange
        var pingMessage = NeptuneMessage.CreatePingMessage(_senderId);

        // Act
        var pongMessage = NeptuneMessage.CreatePongMessage(_recipientId, pingMessage);

        // Assert
        Assert.That(pongMessage, Is.Not.Null);
        Assert.That(pongMessage.Header.SenderId, Is.EqualTo(_recipientId));
        Assert.That(pongMessage.Header.Type, Is.EqualTo(MessageType.PONG));
        Assert.That(pongMessage.Header.EncryptionType, Is.EqualTo(EncryptionType.NONE));
        Assert.That(pongMessage.Routing.Recipient, Is.EqualTo(_senderId));
        Assert.That(pongMessage.Payload.Data, Is.EqualTo(pingMessage.Payload.Data));
    }

    [Test]
    public void CreateErrorMessage_WithValidParameters_CreatesCorrectMessage()
    {
        // Arrange
        string errorMessage = "Invalid message format";
        string errorCode = "E101";

        // Act
        var message = NeptuneMessage.CreateErrorMessage(_senderId, _recipientId, errorMessage, errorCode);

        // Assert
        Assert.That(message, Is.Not.Null);
        Assert.That(message.Header.SenderId, Is.EqualTo(_senderId));
        Assert.That(message.Header.Type, Is.EqualTo(MessageType.ERROR));
        Assert.That(message.Header.EncryptionType, Is.EqualTo(EncryptionType.NONE));
        Assert.That(message.Routing.Recipient, Is.EqualTo(_recipientId));
        Assert.That(message.Payload.Format, Is.EqualTo(PayloadFormat.JSON));

        // Parse the JSON payload
        var json = JsonDocument.Parse(message.Payload.Data);
        Assert.That(json.RootElement.GetProperty("message").GetString(), Is.EqualTo(errorMessage));
        Assert.That(json.RootElement.GetProperty("code").GetString(), Is.EqualTo(errorCode));
    }

    [Test]
    public void ToJson_ValidMessage_SerializesCorrectly()
    {
        // Arrange
        var message = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, "Test message");

        // Act
        string json = NeptuneExtensions.ToJson(message);

        // Assert
        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.Not.Empty);
        Assert.That(json, Does.Contain(_senderId));
        Assert.That(json, Does.Contain(_channelName));
        Assert.That(json, Does.Contain("Test message"));
    }

    [Test]
    public void FromJson_ValidJson_DeserializesCorrectly()
    {
        // Arrange
        var originalMessage = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, "Test message");
        string json = NeptuneExtensions.ToJson(originalMessage);

        // Act
        var deserializedMessage = NeptuneExtensions.FromJson(json);

        // Assert
        Assert.That(deserializedMessage, Is.Not.Null);
        Assert.That(deserializedMessage.Header.SenderId, Is.EqualTo(_senderId));
        Assert.That(deserializedMessage.Routing.Channel, Is.EqualTo(_channelName));
        Assert.That(deserializedMessage.Payload.Data, Is.EqualTo("Test message"));
    }

    [Test]
    public void IsValid_ValidMessage_ReturnsTrue()
    {
        // Arrange
        var message = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, "Test message");

        message.Crypto.ChannelKeyId = "ch-key-2023-04-01";

        // Act
        bool isValid = NeptuneExtensions.IsValid(message);

        // Assert
        Assert.That(isValid, Is.True);
    }

    [Test]
    public void IsValid_MessageWithoutSenderId_ReturnsFalse()
    {
        // Arrange
        var message = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, "Test message");
        message.Header.SenderId = string.Empty;

        // Act
        bool isValid = NeptuneExtensions.IsValid(message);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void Clone_ValidMessage_CreatesIdenticalCopy()
    {
        // Arrange
        var originalMessage = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, "Test message");
        originalMessage.Crypto.ChannelKeyId = "test-key-1";

        // Act
        var clonedMessage = NeptuneExtensions.Clone(originalMessage);

        // Assert
        Assert.That(clonedMessage, Is.Not.Null);
        Assert.That(clonedMessage.Header.SenderId, Is.EqualTo(originalMessage.Header.SenderId));
        Assert.That(clonedMessage.Header.Type, Is.EqualTo(originalMessage.Header.Type));
        Assert.That(clonedMessage.Routing.Channel, Is.EqualTo(originalMessage.Routing.Channel));
        Assert.That(clonedMessage.Crypto.ChannelKeyId, Is.EqualTo(originalMessage.Crypto.ChannelKeyId));
        Assert.That(clonedMessage.Payload.Data, Is.EqualTo(originalMessage.Payload.Data));

        // Make sure it's a deep copy
        clonedMessage.Crypto.ChannelKeyId = "another-key";
        Assert.That(originalMessage.Crypto.ChannelKeyId, Is.Not.EqualTo(clonedMessage.Crypto.ChannelKeyId));
    }
}
