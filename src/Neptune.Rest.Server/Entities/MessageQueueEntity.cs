using FreeSql.DataAnnotations;
using Neptune.Database.Core.Impl.Entities;

namespace Neptune.Rest.Server.Entities;

[Table(Name = "message_queue")]
public class MessageQueueEntity : BaseDbEntity
{
    [Column(StringLength = 64)]
    public string MessageId { get; set; }

    public Guid DestinationUserId { get; set; }

    public Guid? DestinationDeviceId { get; set; }

    [Column(StringLength = -1)] // TEXT/LONGTEXT
    public string MessageData { get; set; }

    public int Priority { get; set; }

    [Column(StringLength = 20)]
    public string Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    // Navigation properties
    [Navigate(nameof(DestinationUserId))]
    public virtual UserEntity DestinationUser { get; set; }

    [Navigate(nameof(DestinationDeviceId))]
    public virtual DeviceEntity DestinationDevice { get; set; }
}
