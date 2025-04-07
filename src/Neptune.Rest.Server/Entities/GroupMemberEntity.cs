using FreeSql.DataAnnotations;
using Neptune.Database.Core.Impl.Entities;

namespace Neptune.Rest.Server.Entities;

[Table(Name = "group_members")]
public class GroupMemberEntity : BaseDbEntity
{
    public Guid GroupId { get; set; }

    public Guid UserId { get; set; }

    [Column(StringLength = 20)]
    public string Role { get; set; }

    public DateTime JoinedAt { get; set; }

    [Column(StringLength = 512)]
    public string EncryptedGroupKey { get; set; }

    // Navigation properties
    [Navigate(nameof(GroupId))]
    public virtual GroupEntity Group { get; set; }

    [Navigate(nameof(UserId))]
    public virtual UserEntity User { get; set; }
}
