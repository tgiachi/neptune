using FreeSql.DataAnnotations;
using Neptune.Database.Core.Impl.Entities;

namespace Neptune.Rest.Server.Entities;

[Table(Name = "users")]
public class UserEntity : BaseDbEntity
{
    [Column(StringLength = 64)] public string NodeId { get; set; }

    [Column(StringLength = 50)] public string Username { get; set; }

    [Column(StringLength = 512)] public string PublicKey { get; set; }

    public DateTime LastSeen { get; set; }

    [Column(StringLength = 20)] public string Status { get; set; }

    [Column(StringLength = -1)] // TEXT/LONGTEXT
    public string ProfileData { get; set; }

    // Navigation properties
    [Navigate(nameof(DeviceEntity.UserId))]
    public virtual List<DeviceEntity> Devices { get; set; }

    [Navigate(nameof(ConnectionEntity.User1Id))]
    public virtual List<ConnectionEntity> OutgoingConnections { get; set; }

    [Navigate(nameof(ConnectionEntity.User2Id))]
    public virtual List<ConnectionEntity> IncomingConnections { get; set; }

    [Navigate(nameof(GroupEntity.CreatedById))]
    public virtual List<GroupEntity> CreatedGroups { get; set; }

    [Navigate(nameof(GroupMemberEntity.UserId))]
    public virtual List<GroupMemberEntity> GroupMemberships { get; set; }

    [Navigate(nameof(MessageQueueEntity.DestinationUserId))]
    public virtual List<MessageQueueEntity> PendingMessages { get; set; }
}
