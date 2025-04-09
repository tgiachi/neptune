using Neptune.Packets.Messages;
using PeterO.Cbor;

namespace Neptune.Transport.Core.Extensions;

public static class NeptuneMessageToCborExtension
{
    public static byte[] ToCbor(this NeptuneMessage message)
    {
        var cbor = CBORObject
            .NewOrderedMap()
            .Add(nameof(message.Id), message.Id)
            .Add(nameof(message.From), message.From)
            .Add(nameof(message.To), message.To)
            .Add(nameof(message.Timestamp), message.Timestamp)
            .Add(nameof(message.Payload), message.Payload)
            .Add(nameof(message.Signature), message.Signature)
            .Add(nameof(message.Hops), message.Hops)
            .Add(nameof(message.MaxHops), message.MaxHops)
            .Add(nameof(message.History), message.History);


        return cbor.EncodeToBytes();
    }

    public static NeptuneMessage FromCbor(this byte[] payload)
    {
        var cbor = CBORObject.DecodeFromBytes(payload);
        var message = new NeptuneMessage
        {
            Id = cbor[nameof(NeptuneMessage.Id)].AsString(),
            From = cbor[nameof(NeptuneMessage.From)].AsString(),
            To = cbor[nameof(NeptuneMessage.To)].AsString(),
            Timestamp = cbor[nameof(NeptuneMessage.Timestamp)].AsInt64(),
            Payload = cbor[nameof(NeptuneMessage.Payload)].AsString(),
            Signature = cbor[nameof(NeptuneMessage.Signature)].AsString(),
            Hops = cbor[nameof(NeptuneMessage.Hops)].AsInt32(),
            MaxHops = cbor[nameof(NeptuneMessage.MaxHops)].AsInt32(),
            History = cbor[nameof(NeptuneMessage.History)].AsString()
        };

        return message;
    }
}
