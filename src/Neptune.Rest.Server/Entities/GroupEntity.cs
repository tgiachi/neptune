using FreeSql.DataAnnotations;
using Neptune.Database.Core.Impl.Entities;

namespace Neptune.Rest.Server.Entities;

[Table(Name = "groups")]
public class GroupEntity : BaseDbEntity
{
    [Column(StringLength = 100)]
    public string Name { get; set; }

    [Column(StringLength = 500)]
    public string Description { get; set; }

    public Guid CreatedById { get; set; }

    [Column(StringLength = 512)]
    public string GroupKey { get; set; }

    // Navigation properties
    [Navigate(nameof(CreatedById))]
    public virtual UserEntity Creator { get; set; }

    [Navigate(nameof(GroupMemberEntity.GroupId))]
    public virtual List<GroupMemberEntity> Members { get; set; }
}
