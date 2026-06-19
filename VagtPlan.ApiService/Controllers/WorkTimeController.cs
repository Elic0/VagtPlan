using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiService.DBContext;
using ApiService.Models;
using ApiService.DTOS;
using ApiService.Helpers;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkTimeController : ControllerBase
    {
        private readonly AppDBContext _context;

        public WorkTimeController(AppDBContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<WorkTime>>> GetWorkTimes()
        {
            var workTimes = await _context.WorkTimes.ToListAsync();
            return Ok(workTimes);
        }

        [Authorize]
        [HttpGet("get/{id}")]
        public async Task<ActionResult<WorkTime>> GetWorkTime([FromRoute] int id)
        {
            var workTime = await _context.WorkTimes.FindAsync(id);

            if (workTime == null)
            {
                return NotFound();
            }

            return Ok(workTime);
        }

        [Authorize]
        [HttpGet("get/byDepartment/{departmentId}")]
        public async Task<ActionResult<IEnumerable<WorkTime>>> GetWorkTimesByDepartment([FromRoute] int departmentId)
        {
            var workTimes = await _context.WorkTimes
                .Where(w => w.DepartmentId == departmentId)
                .OrderBy(w => w.DayOfWeek)
                .ThenBy(w => w.StartTime)
                .ToListAsync();

            return Ok(workTimes);
        }

        [Authorize]
        [HttpPost("createWorkTime")]
        public async Task<ActionResult<WorkTime>> CreateWorkTime(WorkTimeDTO workTimeDTO)
        {
            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == workTimeDTO.DepartmentId);
            if (!departmentExists)
            {
                return BadRequest("Department not found.");
            }

            if (workTimeDTO.EndTime <= workTimeDTO.StartTime)
            {
                return BadRequest("Sluttid skal være efter starttid.");
            }

            var existingForDepartment = await _context.WorkTimes
                .Where(w => w.DepartmentId == workTimeDTO.DepartmentId)
                .ToListAsync();

            if (WorkTimeOverlapHelper.HasOverlap(
                existingForDepartment,
                workTimeDTO.DayOfWeek,
                workTimeDTO.StartTime,
                workTimeDTO.EndTime))
            {
                return BadRequest("Arbejdstiden overlapper med en eksisterende tid på samme ugedag.");
            }

            var workTime = new WorkTime
            {
                DepartmentId = workTimeDTO.DepartmentId,
                StartTime = workTimeDTO.StartTime,
                EndTime = workTimeDTO.EndTime,
                DayOfWeek = workTimeDTO.DayOfWeek
            };

            _context.WorkTimes.Add(workTime);
            await _context.SaveChangesAsync();

            return Ok(workTime);
        }

        [Authorize]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditWorkTime([FromRoute] int id, [FromBody] WorkTimeDTO workTimeDTO)
        {
            var workTime = await _context.WorkTimes.FindAsync(id);
            if (workTime == null)
            {
                return NotFound();
            }

            if (workTimeDTO.EndTime <= workTimeDTO.StartTime)
            {
                return BadRequest("Sluttid skal være efter starttid.");
            }

            var existingForDepartment = await _context.WorkTimes
                .Where(w => w.DepartmentId == workTimeDTO.DepartmentId)
                .ToListAsync();

            if (WorkTimeOverlapHelper.HasOverlap(
                existingForDepartment,
                workTimeDTO.DayOfWeek,
                workTimeDTO.StartTime,
                workTimeDTO.EndTime,
                id))
            {
                return BadRequest("Arbejdstiden overlapper med en eksisterende tid på samme ugedag.");
            }

            workTime.DepartmentId = workTimeDTO.DepartmentId;
            workTime.StartTime = workTimeDTO.StartTime;
            workTime.EndTime = workTimeDTO.EndTime;
            workTime.DayOfWeek = workTimeDTO.DayOfWeek;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkTimeExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return Ok(workTime);
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteWorkTime([FromRoute] int id)
        {
            var workTime = await _context.WorkTimes.FindAsync(id);
            if (workTime == null)
            {
                return NotFound();
            }

            _context.WorkTimes.Remove(workTime);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkTimeExists(int id)
        {
            return _context.WorkTimes.Any(e => e.Id == id);
        }
    }
}
