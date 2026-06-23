using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiService.DBContext;
using ApiService.Models;
using ApiService.DTOS;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDBContext _context;

        public UserController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [Authorize]
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/5
        [Authorize]
        [HttpGet("get/{id}")]
        public async Task<ActionResult<UserDTO>> GetUser([FromRoute] int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [Authorize]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditUser([FromRoute]int id, [FromBody] UserDTO userDTO)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var validationError = await ValidateUserDtoAsync(userDTO);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            user.Name = userDTO.Name.Trim();
            user.VactionDays = userDTO.VactionDays;
            user.DepartmentId = userDTO.DepartmentId;
            user.UserRoleId = userDTO.UserRoleId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }

                throw;
            }
            catch (DbUpdateException)
            {
                return BadRequest("Kunne ikke gemme bruger. Tjek at afdeling og rolle findes.");
            }

            return Ok(user);
        }

        // POST: api/Users
        [Authorize]
        [HttpPost("createUser")]
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO userDTO)
        {
            var validationError = await ValidateUserDtoAsync(userDTO);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var user = new User
            {
                Name = userDTO.Name.Trim(),
                Password = null,
                VactionDays = userDTO.VactionDays,
                DepartmentId = userDTO.DepartmentId,
                UserRoleId = userDTO.UserRoleId
            };
            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Kunne ikke oprette bruger. Tjek at afdeling og rolle findes.");
            }

            return Ok(user);
        }

        // DELETE: api/Users/5
        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private async Task<string?> ValidateUserDtoAsync(UserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.Name))
            {
                return "Navn må ikke være tomt.";
            }

            if (userDTO.DepartmentId <= 0)
            {
                return "Vælg en afdeling.";
            }

            if (!await _context.Departments.AnyAsync(d => d.Id == userDTO.DepartmentId))
            {
                return "Afdelingen findes ikke.";
            }

            if (userDTO.UserRoleId <= 0)
            {
                return "Vælg en rolle.";
            }

            if (!await _context.UserRoles.AnyAsync(r => r.Id == userDTO.UserRoleId))
            {
                return "Rollen findes ikke.";
            }

            if (userDTO.VactionDays < 0)
            {
                return "Feriedage kan ikke være negative.";
            }

            return null;
        }
    }
}
