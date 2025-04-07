using FreeSql.DataAnnotations;
using Neptune.Database.Core.Impl.Entities;

namespace Neptune.Rest.Server.Entities;

[Table(Name = "connections")]
public class ConnectionEntity : BaseDbEntity
{
    public Guid User1Id { get; set; }

    public Guid User2Id { get; set; }

    [Column(StringLength = 20)]
    public string Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    [Navigate(nameof(User1Id))]
    public virtual UserEntity User1 { get; set; }

    [Navigate(nameof(User2Id))]
    public virtual UserEntity User2 { get; set; }
}
