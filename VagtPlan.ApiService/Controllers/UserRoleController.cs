using ApiService.DBContext;
using ApiService.DTOS;
using ApiService.Models;
using k8s.KubeConfigModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRoleController : ControllerBase
    {
        private readonly AppDBContext _context;

        public UserRoleController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/UserRole/get
        [Authorize]
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<UserRoleDTO>>> GetUserRoles()
        {
            var userRoles = await _context.UserRoles.ToListAsync();

            return Ok(userRoles);
        }

        [Authorize]
        [HttpGet("get/{id}")]
        public async Task<ActionResult<UserRoleDTO>> GetUserRole([FromRoute] int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
            {
                return NotFound();
            }
            return Ok(userRole);
        }

        [Authorize]
        [HttpPost("createUserRole")]
        public async Task<ActionResult<UserRoleDTO>> CreateUserRole([FromBody] UserRoleDTO userRoleDTO)
        {
            var userRole = new UserRole
            {
                Name = userRoleDTO.Name
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok(userRole);
        }

        [Authorize]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditUserRole([FromRoute] int id, [FromBody] UserRoleDTO userRoleDTO)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
            {
                return NotFound();
            }
            userRole.Name = userRoleDTO.Name;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUserRole([FromRoute] int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
            {
                return NotFound();
            }
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool UserRoleExists(int id)
        {
            return _context.UserRoles.Any(e => e.Id == id);
        }
    }
}
