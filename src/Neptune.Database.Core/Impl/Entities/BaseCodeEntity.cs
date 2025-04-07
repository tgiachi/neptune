using System.ComponentModel.DataAnnotations;
using Neptune.Database.Core.Interfaces.Entities;

namespace Neptune.Database.Core.Impl.Entities;

public class BaseCodeEntity : BaseDbEntity, ICodeDbEntity
{
    [MaxLength(15)] public string Code { get; set; }
}
