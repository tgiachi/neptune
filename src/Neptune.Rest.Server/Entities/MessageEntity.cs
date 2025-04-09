using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;
using Neptune.Database.Core.Impl.Entities;

namespace Neptune.Rest.Server.Entities;

[System.ComponentModel.DataAnnotations.Schema.Table("messages")]
public class MessageEntity : BaseDbEntity
{
    [Required] [MaxLength(420)] public string From { get; set; }

    [Required] [MaxLength(420)] public string To { get; set; }

    public Guid? ChannelId { get; set; }

    public string Payload { get; set; } // base64 encoded encrypted payload

    public string Signature { get; set; } // base64 encoded digital signature

    public int Timestamp { get; set; }

    public int Hops { get; set; }

    public int MaxHops { get; set; }

    public string History { get; set; } // compact history string: "deviceId:lat,lon;deviceId2:lat,lon"

    public bool IsDelivered { get; set; } = false;

    public bool Inbox { get; set; } = false;

    public bool Outbox { get; set; } = false;
}
