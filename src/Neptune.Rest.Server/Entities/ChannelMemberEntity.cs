using System.ComponentModel.DataAnnotations.Schema;
using Neptune.Database.Core.Impl.Entities;
using Neptune.Rest.Server.Types;

namespace Neptune.Rest.Server.Entities;

[Table("channel_members")]
public class ChannelMemberEntity : BaseDbEntity
{
    public Guid ChannelId { get; set; }

    [ForeignKey("ChannelId")] public virtual ChannelEntity Channel { get; set; } = null!;

    public Guid UserId { get; set; }

    [ForeignKey("UserId")] public virtual UserEntity User { get; set; } = null!;

    public ChannelUserRoleType Role { get; set; } = ChannelUserRoleType.Member;
}
