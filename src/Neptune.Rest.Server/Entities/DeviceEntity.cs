using FreeSql.DataAnnotations;
using Neptune.Database.Core.Impl.Entities;

namespace Neptune.Rest.Server.Entities;

[Table(Name = "devices")]
public class DeviceEntity : BaseDbEntity
{
    public Guid UserId { get; set; }

    [Column(StringLength = 30)]
    public string DeviceType { get; set; }

    [Column(StringLength = 100)]
    public string DeviceName { get; set; }

    [Column(StringLength = 512)]
    public string PublicKey { get; set; }

    public DateTime LastConnected { get; set; }

    [Column(StringLength = -1)] // TEXT/LONGTEXT
    public string Capabilities { get; set; }

    [Column(StringLength = 45)]
    public string RegistrationIp { get; set; }

    [Column(StringLength = 20)]
    public string ClientVersion { get; set; }

    // Navigation property
    [Navigate(nameof(UserId))]
    public virtual UserEntity User { get; set; }

    [Navigate(nameof(MessageQueueEntity.DestinationDeviceId))]
    public virtual List<MessageQueueEntity> DeviceSpecificMessages { get; set; }
}
