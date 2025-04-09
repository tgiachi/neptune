namespace Neptune.Server.Core.Data.Internal;

public record OutgoingMessageData(string From, string To, string MessageId, string Message, string SourceNodeId);
