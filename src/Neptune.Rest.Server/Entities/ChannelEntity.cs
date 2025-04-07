using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Neptune.Database.Core.Impl.Entities;

namespace Neptune.Rest.Server.Entities;

[Table("channels")]
public class ChannelEntity : BaseDbEntity
{
    [MaxLength(250)] public string Name { get; set; }

    [MaxLength(200)] public string NodeHostName { get; set; }

    public bool IsPublic { get; set; } = false;

    public Guid CreatedByUserId { get; set; }

    [ForeignKey("CreatedByUserId")]
    public virtual UserEntity CreatedByUser { get; set; } = null!;
}
