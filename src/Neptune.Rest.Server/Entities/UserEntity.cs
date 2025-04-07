using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Neptune.Database.Core.Impl.Entities;

namespace Neptune.Rest.Server.Entities;


[Table("users")]
public class UserEntity : BaseDbEntity
{
    [MaxLength(200)]
    public string Username { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    [MaxLength(200)]
    public string NodeHostName { get; set; }

    public string PublicKey { get; set; }
}
