using System.ComponentModel.DataAnnotations;

namespace Neptune.Server.Core.Data.Rest;

public class RegisterRequestObject
{

    [Required]
    public string Username { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }
}
