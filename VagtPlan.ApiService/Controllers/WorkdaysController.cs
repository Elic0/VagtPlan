using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiService.DTOS;
using ApiService.Models;
using ApiService.DBContext;

[Route("api/[controller]")]
[ApiController]
public class WorkdaysController : ControllerBase
{
    private readonly AppDBContext _context;
    public WorkdaysController(AppDBContext context)
    {
        _context = context;
    }

    // GET: api/Workday
    [HttpGet("get")]
    public async Task<ActionResult<IEnumerable<WorkdayDTO>>> GetWorkdays()
    {
        var WorkDay = await _context.Workdays.ToListAsync();

        return Ok(WorkDay);
    }

    // GET: api/Workday/5
    [HttpGet("get/{id}")]
    public async Task<ActionResult<WorkdayDTO>> GetWorkday(int id)
    {
        var workday = await _context.Workdays.FindAsync(id);

        if (workday == null)
        {
            return NotFound();
        }

        return Ok(workday);
    }

    // PUT: api/Workday/5
    [HttpPut("edit/{id}")]
    public async Task<IActionResult> EditWorkday([FromRoute] int id, [FromBody] WorkdayDTO workdayDTO)
    {
        var workday = await _context.Workdays.FindAsync(id);
        if (workday == null)
        {
            return NotFound();
        }

        workday.Date = workdayDTO.Date;
        workday.StartTime = workdayDTO.StartTime;
        workday.EndTime = workdayDTO.EndTime;
        workday.UserId = workdayDTO.UserId;
        workday.StatusId = workdayDTO.StatusId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!WorkdayExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Ok(workday);
    }

    // POST: api/Workday
    [HttpPost("createWorkDay")]
    public async Task<ActionResult<Workday>> CreateWorkday(WorkdayDTO workdayDTO)
    {
        var workday = new Workday
        {
            Date = workdayDTO.Date,
            StartTime = workdayDTO.StartTime,
            EndTime = workdayDTO.EndTime,
            UserId = workdayDTO.UserId,
            StatusId = workdayDTO.StatusId
        };
        _context.Workdays.Add(workday);
        await _context.SaveChangesAsync();

        return Ok(workday);
    }

    // DELETE: api/Workday/5
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteWorkday(int? id)
    {
        var workday = await _context.Workdays.FindAsync(id);
        if (workday == null)
        {
            return NotFound();
        }

        _context.Workdays.Remove(workday);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool WorkdayExists(int? id)
    {
        return _context.Workdays.Any(e => e.Id == id);
    }
}
