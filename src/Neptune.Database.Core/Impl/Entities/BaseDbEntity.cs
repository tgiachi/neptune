using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;
using Neptune.Database.Core.Interfaces.Entities;

namespace Neptune.Database.Core.Impl.Entities;

public class BaseDbEntity : IDbEntity
{
    [Column(IsIdentity = true, IsPrimary = true)]
    [Key] public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
