using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiService.Models;
using Microsoft.IdentityModel.Tokens;

namespace ApiService.Services;

public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
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

        var roleName = user.UserRole?.Name ?? "User";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Name),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, roleName),
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
