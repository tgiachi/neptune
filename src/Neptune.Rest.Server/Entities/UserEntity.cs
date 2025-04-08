using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;
using Neptune.Database.Core.Impl.Entities;

namespace Neptune.Rest.Server.Entities;

[System.ComponentModel.DataAnnotations.Schema.Table("users")]
public class UserEntity : BaseDbEntity
{
    [MaxLength(200)]
    [Column(IsNullable = false)]
    public string Username { get; set; }

    [Column(IsNullable = false)] public string Email { get; set; }

    [Column(IsNullable = false)] public string PasswordHash { get; set; }

    [MaxLength(200)]
    [Column(IsNullable = false)]
    public string NodeHostName { get; set; }

    [Column(IsNullable = false)] public string PublicKey { get; set; }

    [Navigate(nameof(ChannelMemberEntity.UserId))]
    public List<ChannelMemberEntity> Channels { get; set; }
}
