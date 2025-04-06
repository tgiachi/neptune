using Neptune.Packets.Extensions;

namespace Neptune.Tests;

[TestFixture]
public class NeptuneExtensionsTests
{
    [Test]
    public void FormatNeptuneId_ValidComponents_ReturnsCorrectFormat()
    {
        // Arrange
        string localId = "squid";
        string server = "server.neptune.io";
        string expected = "squid@server.neptune.io";

        // Act
        string result = NeptuneExtensions.FormatNeptuneId(localId, server);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void FormatNeptuneId_EmptyLocalId_ThrowsArgumentNullException()
    {
        // Arrange
        string localId = "";
        string server = "server.neptune.io";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => NeptuneExtensions.FormatNeptuneId(localId, server));
    }

    [Test]
    public void FormatNeptuneId_EmptyServer_ThrowsArgumentNullException()
    {
        // Arrange
        string localId = "squid";
        string server = "";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => NeptuneExtensions.FormatNeptuneId(localId, server));
    }

    [Test]
    public void TryParseNeptuneId_ValidId_ParsesCorrectly()
    {
        // Arrange
        string id = "squid@server.neptune.io";
        string expectedLocalId = "squid";
        string expectedServer = "server.neptune.io";

        // Act
        bool result = NeptuneExtensions.TryParseNeptuneId(id, out string localId, out string server);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(localId, Is.EqualTo(expectedLocalId));
        Assert.That(server, Is.EqualTo(expectedServer));
    }

    [Test]
    public void TryParseNeptuneId_NoAtSymbol_ReturnsFalse()
    {
        // Arrange
        string id = "squidserver.neptune.io";

        // Act
        bool result = NeptuneExtensions.TryParseNeptuneId(id, out string localId, out string server);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(localId, Is.Empty);
        Assert.That(server, Is.Empty);
    }

    [Test]
    public void TryParseNeptuneId_EmptyLocalId_ReturnsFalse()
    {
        // Arrange
        string id = "@server.neptune.io";

        // Act
        bool result = NeptuneExtensions.TryParseNeptuneId(id, out string localId, out string server);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(localId, Is.Empty);
        Assert.That(server, Is.Empty);
    }

    [Test]
    public void TryParseNeptuneId_EmptyServer_ReturnsFalse()
    {
        // Arrange
        string id = "squid@";

        // Act
        bool result = NeptuneExtensions.TryParseNeptuneId(id, out string localId, out string server);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(localId, Is.Empty);
        Assert.That(server, Is.Empty);
    }

    [Test]
    public void TryParseNeptuneId_NullId_ReturnsFalse()
    {
        // Arrange
        string id = null;

        // Act
        bool result = NeptuneExtensions.TryParseNeptuneId(id, out string localId, out string server);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(localId, Is.Empty);
        Assert.That(server, Is.Empty);
    }

    [Test]
    public void IsValidChannelName_ValidLocalChannel_ReturnsTrue()
    {
        // Arrange
        string channelName = "#general";

        // Act
        bool result = NeptuneExtensions.IsValidChannelName(channelName);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsValidChannelName_ValidGlobalChannel_ReturnsTrue()
    {
        // Arrange
        string channelName = "##neptune";

        // Act
        bool result = NeptuneExtensions.IsValidChannelName(channelName);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsValidChannelName_NoHashPrefix_ReturnsFalse()
    {
        // Arrange
        string channelName = "general";

        // Act
        bool result = NeptuneExtensions.IsValidChannelName(channelName);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValidChannelName_TooShort_ReturnsFalse()
    {
        // Arrange
        string channelName = "#"; // Just the hash, no name

        // Act
        bool result = NeptuneExtensions.IsValidChannelName(channelName);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValidChannelName_TooLong_ReturnsFalse()
    {
        // Arrange
        string channelName = "#" + new string('a', 32); // 33 characters (1 hash + 32 'a's)

        // Act
        bool result = NeptuneExtensions.IsValidChannelName(channelName);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValidChannelName_InvalidCharacters_ReturnsFalse()
    {
        // Arrange
        string channelName = "#general!"; // Contains exclamation mark

        // Act
        bool result = NeptuneExtensions.IsValidChannelName(channelName);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsValidChannelName_ValidSpecialCharacters_ReturnsTrue()
    {
        // Arrange
        string channelName = "#general-chat_123";

        // Act
        bool result = NeptuneExtensions.IsValidChannelName(channelName);

        // Assert
        Assert.That(result, Is.True);
    }
}
