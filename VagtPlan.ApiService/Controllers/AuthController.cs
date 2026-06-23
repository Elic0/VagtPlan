using ApiService.DBContext;
using ApiService.DTOS;
using ApiService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly JwtService _jwtService;
    private readonly IConfiguration _configuration;

    public AuthController(AppDBContext context, JwtService jwtService, IConfiguration configuration)
    {
        _context = context;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO request)
    {
        var username = request.Username?.Trim() ?? string.Empty;
        var password = request.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return Unauthorized("Invalid username or password.");
        }

        var user = await _context.Users
            .Include(u => u.UserRole)
            .FirstOrDefaultAsync(u => u.Name.ToLower() == username.ToLower());

        if (user is null || !PasswordService.VerifyPassword(password, user.Password))
        {
            return Unauthorized("Invalid username or password.");
        }

        var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var configuredMinutes)
            ? configuredMinutes
            : 120;

        var token = _jwtService.GenerateToken(user);

        return Ok(new LoginResponseDTO
        {
            Token = token,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes)
        });
    }
}
