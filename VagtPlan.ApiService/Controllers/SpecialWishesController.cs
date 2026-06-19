using ApiService.DBContext;
using ApiService.DTOS;
using ApiService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    [Authorize]
    [HttpGet("get")]
    public async Task<ActionResult<IEnumerable<SpecialWishDTO>>> GetSpecialWishes()
    {
        var specialWishes = await _context.SpecialWishes.ToListAsync();

        return Ok(specialWishes);
    }

    // GET: api/SpecialWish/5
    [Authorize]
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
    [Authorize]
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
            // Check for overlaps with other wishes for the same user/day/status
            var startDate = specialWishDTO.StartDate;
            var endDate = specialWishDTO.EndDate;

            var conflicting = await _context.SpecialWishes
                .Where(sw => sw.UserId == specialWishDTO.UserId && sw.DayOfWeek == specialWishDTO.DayOfWeek && sw.StatusId == specialWishDTO.StatusId && sw.Id != id)
                .ToListAsync();

            foreach (var c in conflicting)
            {
                if ((c.StartDate == null && c.EndDate == null) || DateRangesOverlap(c.StartDate, c.EndDate, startDate, endDate))
                {
                    return BadRequest("A conflicting special wish already exists for this user, day and status.");
                }
            }

            specialWish.StartDate = startDate;
            specialWish.EndDate = endDate;
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
    [Authorize]
    [HttpPost("createSpecialWish")]
    public async Task<ActionResult<SpecialWish>> CreateSpecialWish(SpecialWishDTO specialWishDTO)
    {
        // If status is default, do not set dates (leave null)
        var status = await _context.Statuses.FindAsync(specialWishDTO.StatusId);

        DateOnly? start = null;
        DateOnly? end = null;

        if (status == null || !status.Default)
        {
            start = specialWishDTO.StartDate;
            end = specialWishDTO.EndDate;
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
        else
        {
            // For non-default statuses ensure there is no overlapping wish with same user/day/status
            var existing = await _context.SpecialWishes
                .Where(sw => sw.UserId == specialWishDTO.UserId && sw.DayOfWeek == specialWishDTO.DayOfWeek && sw.StatusId == specialWishDTO.StatusId)
                .ToListAsync();

            foreach (var e in existing)
            {
                if ((e.StartDate == null && e.EndDate == null) || DateRangesOverlap(e.StartDate, e.EndDate, start, end))
                {
                    return BadRequest("A conflicting special wish already exists for this user, day and status.");
                }
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
    [Authorize]
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

    private static bool DateRangesOverlap(DateOnly? aStart, DateOnly? aEnd, DateOnly? bStart, DateOnly? bEnd)
    {
        // If any range is open (missing start or end) we cannot reliably compare here; treat as non-overlapping
        if (!aStart.HasValue || !aEnd.HasValue || !bStart.HasValue || !bEnd.HasValue)
            return false;

        return aStart.Value <= bEnd.Value && bStart.Value <= aEnd.Value;
    }
}
