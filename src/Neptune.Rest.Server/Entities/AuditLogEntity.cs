using System.ComponentModel.DataAnnotations.Schema;
using Neptune.Database.Core.Impl.Entities;
using Neptune.Rest.Server.Types;

namespace Neptune.Rest.Server.Entities;

[Table("audit_logs")]
public class AuditLogEntity : BaseDbEntity
{

    public virtual MessageEntity Message { get; set; }
    public Guid MessageId { get; set; }

    public AuditActionType Action { get; set; }

    public string DeviceId { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public DateTime Timestamp { get; set; }
}
