using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FreeSql.DataAnnotations;
using Neptune.Database.Core.Impl.Entities;

namespace Neptune.Rest.Server.Entities;

[System.ComponentModel.DataAnnotations.Schema.Table("channels")]
public class ChannelEntity : BaseDbEntity
{
    [MaxLength(250)] public string Name { get; set; }

    [MaxLength(200)] public string NodeHostName { get; set; }

    public bool IsPublic { get; set; } = false;

    public Guid CreatedByUserId { get; set; }

    [ForeignKey("CreatedByUserId")]
    public virtual UserEntity CreatedByUser { get; set; } = null!;

    [Navigate(nameof(ChannelMemberEntity.ChannelId))]
    public List<ChannelMemberEntity> Members { get; set; }
}
