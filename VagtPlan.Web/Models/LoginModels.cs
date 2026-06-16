using System.ComponentModel.DataAnnotations;

namespace VagtPlan.Web.Models;

public class LoginRequestModel
{
    [Required(ErrorMessage = "Brugernavn er påkrævet.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adgangskode er påkrævet.")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseModel
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
