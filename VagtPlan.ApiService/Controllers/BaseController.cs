using ApiService.DBContext;
using ApiService.DTOS;
using ApiService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly AppDBContext _context;
        public BaseController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet("health")]
        public async Task<IActionResult> GetHealth()
        {
            try
            {
                // Simple health check: verify DB connectivity
                var canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok(new { status = "Healthy" });
                }

                return StatusCode(503, new { status = "Unhealthy", detail = "Database unreachable" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Error", detail = ex.Message });
            }
        }
    }
}
