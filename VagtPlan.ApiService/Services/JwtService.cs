using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiService.DTOS;
using Microsoft.IdentityModel.Tokens;

namespace ApiService.Services;

public class JwtService
{
    private const string DefaultAdminUsername = "Admin";
    private const string DefaultAdminPassword = "Password1!";

    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool ValidateAdminCredentials(LoginRequestDTO request)
    {
        var configuredUsername = _configuration["AdminAuth:Username"] ?? DefaultAdminUsername;
        var configuredPassword = _configuration["AdminAuth:Password"] ?? DefaultAdminPassword;

        var providedUsername = request.Username?.Trim() ?? string.Empty;
        var providedPassword = request.Password?.Trim() ?? string.Empty;

        return string.Equals(providedUsername, configuredUsername, StringComparison.OrdinalIgnoreCase)
            && string.Equals(providedPassword, configuredPassword, StringComparison.Ordinal);
    }

    public string GenerateAdminToken()
    {
        var jwtSecretKey = _configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("JWT secret key not configured.");
        var jwtIssuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT issuer not configured.");
        var jwtAudience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT audience not configured.");
        var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var configuredMinutes)
            ? configuredMinutes
            : 120;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "admin"),
            new(JwtRegisteredClaimNames.UniqueName, DefaultAdminUsername),
            new(ClaimTypes.Name, DefaultAdminUsername),
            new(ClaimTypes.Role, "Admin"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
