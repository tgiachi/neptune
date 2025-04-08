using System.Threading.Channels;
using FreeSql.DataAnnotations;
using Neptune.Database.Core.Impl.Entities;
using Neptune.Rest.Server.Types;

namespace Neptune.Rest.Server.Entities;

[System.ComponentModel.DataAnnotations.Schema.Table("channel_members")]
public class ChannelMemberEntity : BaseDbEntity
{
    [Column(IsNullable = false)] public Guid ChannelId { get; set; }


    [Navigate(nameof(ChannelId))] public ChannelEntity Channel { get; set; }

    [Column(IsNullable = false)] public Guid UserId { get; set; }

    [Navigate(nameof(UserId))] public UserEntity User { get; set; }

    public ChannelUserRoleType Role { get; set; } = ChannelUserRoleType.Member;
}
