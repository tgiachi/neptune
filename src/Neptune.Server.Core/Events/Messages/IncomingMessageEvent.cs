namespace Neptune.Server.Core.Events.Messages;

public record IncomingMessageEvent(string From, string To, string MessageId, string Message, string SourceNodeId);
