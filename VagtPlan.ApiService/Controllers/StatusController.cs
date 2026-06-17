using ApiService.DBContext;
using ApiService.DTOS;
using ApiService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class StatusController : ControllerBase
{
    private readonly AppDBContext _context;
    public StatusController(AppDBContext context)
    {
        _context = context;
    }

    // GET: api/Status
    [HttpGet("get")]
    public async Task<ActionResult<IEnumerable<StatusDTO>>> GetStatuses()
    {
        var statuses = await _context.Statuses.ToListAsync();
        
        return Ok(statuses);
    }

    // GET: api/Status/5
    [HttpGet("get/{id}")]
    public async Task<ActionResult<StatusDTO>> GetStatus(int id)
    {
        var status = await _context.Statuses.FindAsync(id);

        if (status == null)
        {
            return NotFound();
        }

        return Ok(status);
    }

    // PUT: api/Status/5
    [HttpPut("edit/{id}")]
    public async Task<IActionResult> EditStatus([FromRoute] int id, [FromBody] StatusDTO statusDTO)
    {
        var status = await _context.Statuses.FindAsync(id);
        if (status == null)
        {
            return NotFound();
        }

        status.Name = statusDTO.Name;
        status.Colour = statusDTO.Colour;
        status.IsAvailable = statusDTO.IsAvailable;
        status.Default = statusDTO.Default;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StatusExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Ok(status);
    }

    // POST: api/Status
    [HttpPost("createStatus")]
    public async Task<ActionResult<Status>> CreateStatus(StatusDTO statusDTO)
    {
        var status = new Status
        {
            Name = statusDTO.Name,
            Colour = statusDTO.Colour,
            IsAvailable = statusDTO.IsAvailable,
            Default = statusDTO.Default
        };
        _context.Statuses.Add(status);
        await _context.SaveChangesAsync();

        return Ok(status);
    }

    // DELETE: api/Status/5
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteStatus(int? id)
    {
        var status = await _context.Statuses.FindAsync(id);
        if (status == null)
        {
            return NotFound();
        }

        _context.Statuses.Remove(status);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool StatusExists(int? id)
    {
        return _context.Statuses.Any(e => e.Id == id);
    }
}
