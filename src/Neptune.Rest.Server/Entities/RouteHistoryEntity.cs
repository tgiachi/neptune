using FreeSql.DataAnnotations;
using Neptune.Database.Core.Impl.Entities;

namespace Neptune.Rest.Server.Entities;

[Table(Name = "route_history")]
public class RouteHistoryEntity : BaseDbEntity
{
    [Column(StringLength = 64)] public string MessageId { get; set; }

    [Column(StringLength = -1)] // TEXT/LONGTEXT
    public string NodePath { get; set; }

    public int HopCount { get; set; }

    public long Duration { get; set; }
}
