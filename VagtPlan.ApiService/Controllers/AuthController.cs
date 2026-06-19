using ApiService.DTOS;
using ApiService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly IConfiguration _configuration;

    public AuthController(JwtService jwtService, IConfiguration configuration)
    {
        _jwtService = jwtService;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<LoginResponseDTO> Login([FromBody] LoginRequestDTO request)
    {
        if (!_jwtService.ValidateAdminCredentials(request))
        {
            return Unauthorized("Invalid username or password.");
        }

        var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var configuredMinutes)
            ? configuredMinutes
            : 120;

        var token = _jwtService.GenerateAdminToken();

        return Ok(new LoginResponseDTO
        {
            Token = token,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes)
        });
    }
}
