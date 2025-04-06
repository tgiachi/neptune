using Neptune.Packets.Extensions;
using Neptune.Packets.Messages;

namespace Neptune.Tests;

[TestFixture]
public class NeptuneMessageValidationTests
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
    public void CreateChannelMessage_NullSenderId_ThrowsArgumentNullException()
    {
        // Arrange
        string senderId = null;
        string channelName = "#general";
        string text = "Test message";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreateChannelMessage(senderId, channelName, text)
        );
    }

    [Test]
    public void CreateChannelMessage_EmptySenderId_ThrowsArgumentNullException()
    {
        // Arrange
        string senderId = "";
        string channelName = "#general";
        string text = "Test message";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreateChannelMessage(senderId, channelName, text)
        );
    }

    [Test]
    public void CreateChannelMessage_NullChannelName_ThrowsArgumentNullException()
    {
        // Arrange
        string channelName = null;
        string text = "Test message";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreateChannelMessage(_senderId, channelName, text)
        );
    }

    [Test]
    public void CreateChannelMessage_EmptyChannelName_ThrowsArgumentNullException()
    {
        // Arrange
        string channelName = "";
        string text = "Test message";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreateChannelMessage(_senderId, channelName, text)
        );
    }

    [Test]
    public void CreatePrivateMessage_NullSenderId_ThrowsArgumentNullException()
    {
        // Arrange
        string senderId = null;
        string text = "Test message";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreatePrivateMessage(senderId, _recipientId, text)
        );
    }

    [Test]
    public void CreatePrivateMessage_NullRecipientId_ThrowsArgumentNullException()
    {
        // Arrange
        string recipientId = null;
        string text = "Test message";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreatePrivateMessage(_senderId, recipientId, text)
        );
    }

    [Test]
    public void CreateJoinMessage_NullSenderId_ThrowsArgumentNullException()
    {
        // Arrange
        string senderId = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreateJoinMessage(senderId, _channelName)
        );
    }

    [Test]
    public void CreateJoinMessage_NullChannelName_ThrowsArgumentNullException()
    {
        // Arrange
        string channelName = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreateJoinMessage(_senderId, channelName)
        );
    }

    [Test]
    public void CreateLeaveMessage_NullSenderId_ThrowsArgumentNullException()
    {
        // Arrange
        string senderId = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreateLeaveMessage(senderId, _channelName)
        );
    }

    [Test]
    public void CreateLeaveMessage_NullChannelName_ThrowsArgumentNullException()
    {
        // Arrange
        string channelName = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreateLeaveMessage(_senderId, channelName)
        );
    }

    [Test]
    public void CreatePingMessage_NullSenderId_ThrowsArgumentNullException()
    {
        // Arrange
        string senderId = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreatePingMessage(senderId)
        );
    }

    [Test]
    public void CreatePongMessage_NullSenderId_ThrowsArgumentNullException()
    {
        // Arrange
        string senderId = null;
        var pingMessage = NeptuneMessage.CreatePingMessage(_senderId);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreatePongMessage(senderId, pingMessage)
        );
    }

    [Test]
    public void CreatePongMessage_NullPingMessage_ThrowsArgumentNullException()
    {
        // Arrange
        NeptuneMessage pingMessage = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreatePongMessage(_senderId, pingMessage)
        );
    }

    [Test]
    public void CreatePongMessage_NonPingMessage_ThrowsArgumentException()
    {
        // Arrange
        var nonPingMessage = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, "Not a ping");

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () =>
                NeptuneMessage.CreatePongMessage(_senderId, nonPingMessage)
        );
    }

    [Test]
    public void CreateErrorMessage_NullSenderId_ThrowsArgumentNullException()
    {
        // Arrange
        string senderId = null;
        string errorMessage = "Test error";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreateErrorMessage(senderId, _recipientId, errorMessage)
        );
    }

    [Test]
    public void CreateErrorMessage_NullRecipientId_ThrowsArgumentNullException()
    {
        // Arrange
        string recipientId = null;
        string errorMessage = "Test error";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreateErrorMessage(_senderId, recipientId, errorMessage)
        );
    }

    [Test]
    public void CreateErrorMessage_NullErrorMessage_ThrowsArgumentNullException()
    {
        // Arrange
        string errorMessage = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () =>
                NeptuneMessage.CreateErrorMessage(_senderId, _recipientId, errorMessage)
        );
    }

    [Test]
    public void IsValid_ChannelMessageWithoutChannel_ReturnsFalse()
    {
        // Arrange
        var message = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, "Test message");
        message.Routing.Channel = null;

        // Act
        bool isValid = NeptuneExtensions.IsValid(message);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void IsValid_PrivateMessageWithoutRecipient_ReturnsFalse()
    {
        // Arrange
        var message = NeptuneMessage.CreatePrivateMessage(_senderId, _recipientId, "Test message");
        message.Routing.Recipient = null;

        // Act
        bool isValid = NeptuneExtensions.IsValid(message);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void IsValid_E2EMessageWithoutPublicKey_ReturnsFalse()
    {
        // Arrange
        var message = NeptuneMessage.CreatePrivateMessage(_senderId, _recipientId, "Test message");
        message.Crypto.PublicKey = null;

        // Act
        bool isValid = NeptuneExtensions.IsValid(message);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void IsValid_ChannelEncryptionWithoutChannelKeyId_ReturnsFalse()
    {
        // Arrange
        var message = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, "Test message");
        message.Crypto.ChannelKeyId = null;

        // Act
        bool isValid = NeptuneExtensions.IsValid(message);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void IsValid_MessageWithoutVersion_ReturnsFalse()
    {
        // Arrange
        var message = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, "Test message");
        message.Version = null;

        // Act
        bool isValid = NeptuneExtensions.IsValid(message);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void IsValid_MessageWithoutMessageId_ReturnsFalse()
    {
        // Arrange
        var message = NeptuneMessage.CreateChannelMessage(_senderId, _channelName, "Test message");
        message.Header.MessageId = null;

        // Act
        bool isValid = NeptuneExtensions.IsValid(message);

        // Assert
        Assert.That(isValid, Is.False);
    }
}
