namespace Neptune.Server.Core.Events.Messages;

public class SendMessageQueueEvent
{
    public string MessageId { get; set; }

    public string Payload { get; set; }
}
