using ApiService.DBContext;
using ApiService.DTOS;
using ApiService.Models;
using k8s.KubeConfigModels;
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
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<UserRoleDTO>>> GetUserRoles()
        {
            var userRoles = await _context.UserRoles.ToListAsync();

            return Ok(userRoles);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<UserRoleDTO>> GetUserRole([FromRoute] long id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
            {
                return NotFound();
            }
            return Ok(userRole);
        }

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

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditUserRole([FromRoute] long id, [FromBody] UserRoleDTO userRoleDTO)
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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUserRole([FromRoute] long id)
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

        private bool UserRoleExists(long id)
        {
            return _context.UserRoles.Any(e => e.Id == id);
        }
    }
}
