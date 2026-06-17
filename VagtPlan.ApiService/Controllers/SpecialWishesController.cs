using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiService.DTOS;
using ApiService.Models;
using ApiService.DBContext;

[Route("api/[controller]")]
[ApiController]
public class SpecialWishesController : ControllerBase
{
    private readonly AppDBContext _context;
    public SpecialWishesController(AppDBContext context)
    {
        _context = context;
    }

    // GET: api/SpecialWish
    [HttpGet("get")]
    public async Task<ActionResult<IEnumerable<SpecialWishDTO>>> GetSpecialWishes()
    {
        var specialWishes = await _context.SpecialWishes.ToListAsync();

        return Ok(specialWishes);
    }

    // GET: api/SpecialWish/5
    [HttpGet("get/{id}")]
    public async Task<ActionResult<SpecialWishDTO>> GetSpecialWish(int id)
    {
        var specialWish = await _context.SpecialWishes.FindAsync(id);

        if (specialWish == null)
        {
            return NotFound();
        }

        return Ok(specialWish);
    }

    // PUT: api/SpecialWish/5
    [HttpPut("edit/{id}")]
    public async Task<IActionResult> EditSpecialWish([FromRoute] int id, [FromBody] SpecialWishDTO specialWishDTO)
    {
        var specialWish = await _context.SpecialWishes.FindAsync(id);
        if (specialWish == null)
        {
            return NotFound();
        }

        // If the selected status is marked as Default, clear dates
        var status = await _context.Statuses.FindAsync(specialWishDTO.StatusId);
        if (status != null && status.Default)
        {
            specialWish.StartDate = null;
            specialWish.EndDate = null;
        }
        else
        {
            specialWish.StartDate = specialWishDTO.StartDate?.ToUniversalTime();
            specialWish.EndDate = specialWishDTO.EndDate?.ToUniversalTime();
        }

        specialWish.DayOfWeek = specialWishDTO.DayOfWeek;
        specialWish.UserId = specialWishDTO.UserId;
        specialWish.StatusId = specialWishDTO.StatusId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SpecialWishExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Ok(specialWish);
    }

    // POST: api/SpecialWish
    [HttpPost("createSpecialWish")]
    public async Task<ActionResult<SpecialWish>> CreateSpecialWish(SpecialWishDTO specialWishDTO)
    {
        // If status is default, do not set dates (leave null)
        var status = await _context.Statuses.FindAsync(specialWishDTO.StatusId);

        DateTime? start = null;
        DateTime? end = null;

        if (status == null || !status.Default)
        {
            start = specialWishDTO.StartDate?.ToUniversalTime();
            end = specialWishDTO.EndDate?.ToUniversalTime();
        }
        // If status is default, overwrite (remove) any existing wishes for this user/day
        if (status != null && status.Default)
        {
            var existing = await _context.SpecialWishes
                .Where(sw => sw.UserId == specialWishDTO.UserId && sw.DayOfWeek == specialWishDTO.DayOfWeek)
                .ToListAsync();

            if (existing.Any())
            {
                _context.SpecialWishes.RemoveRange(existing);
            }
        }

        var specialWish = new SpecialWish
        {
            StartDate = start,
            EndDate = end,
            DayOfWeek = specialWishDTO.DayOfWeek,
            UserId = specialWishDTO.UserId,
            StatusId = specialWishDTO.StatusId
        };
        _context.SpecialWishes.Add(specialWish);
        await _context.SaveChangesAsync();

        return Ok(specialWish);
    }

    // DELETE: api/SpecialWish/5
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteSpecialWish(int? id)
    {
        var specialwish = await _context.SpecialWishes.FindAsync(id);
        if (specialwish == null)
        {
            return NotFound();
        }

        _context.SpecialWishes.Remove(specialwish);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SpecialWishExists(int? id)
    {
        return _context.SpecialWishes.Any(e => e.Id == id);
    }
}
