using Neptune.Packets.Messages;
using Neptune.Rest.Server.Entities;

namespace Neptune.Rest.Server.Extensions;

public static  class MessageEntityToNeptuneMessageExtension
{

    public static NeptuneMessage ToNeptuneMessage(this MessageEntity messageEntity)
    {
        return new NeptuneMessage
        {
            From = messageEntity.From,
            To = messageEntity.To,
            Id = messageEntity.Id.ToString(),
            Hops = messageEntity.Hops,
            MaxHops = messageEntity.MaxHops,
            Payload = messageEntity.Payload,
            Signature = messageEntity.Signature,
            Timestamp = messageEntity.Timestamp,
            History = messageEntity.History
        };
    }

}
