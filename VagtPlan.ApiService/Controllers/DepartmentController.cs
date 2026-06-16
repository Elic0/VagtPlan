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
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly AppDBContext _context;

        public DepartmentController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            var departments = await _context.Departments.ToListAsync();
            return Ok(departments);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Department>> GetDepartment([FromRoute] long id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }

        [HttpPost("createDepartment")]
        public async Task<ActionResult<Department>> CreateDepartment(DepartmentDTO departmentDTO)
        {
            var department = new Department
            {
                Name = departmentDTO.Name
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return Ok(department);
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditDepartment([FromRoute] long id, [FromBody] DepartmentDTO departmentDTO)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            department.Name = departmentDTO.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return Ok(department);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDepartment([FromRoute] long id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            var workTimes = await _context.WorkTimes
                .Where(w => w.DepartmentId == id)
                .ToListAsync();

            _context.WorkTimes.RemoveRange(workTimes);
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(long id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
