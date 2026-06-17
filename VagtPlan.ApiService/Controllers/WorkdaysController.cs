using ApiService.DBContext;
using ApiService.DTOS;
using ApiService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class WorkdaysController : ControllerBase
{
    private readonly AppDBContext _context;
    public WorkdaysController(AppDBContext context)
    {
        _context = context;
    }

    public class GenerateWorkdaysRequest
    {
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public required List<int> SelectedUserIds { get; set; }
        public required int DepartmentId { get; set; }
    }

    public class GenerateWorkdaysResult
    {
        public List<WorkdayDTO> Created { get; set; } = new();
        public Dictionary<int,int> AssignedCounts { get; set; } = new();
    }

    // GET: api/Workday
    [Authorize]
    [HttpGet("get")]
    public async Task<ActionResult<IEnumerable<WorkdayDTO>>> GetWorkdays()
    {
        var WorkDay = await _context.Workdays.ToListAsync();

        return Ok(WorkDay);
    }

    // POST: api/Workday/generate
    [Authorize]
    [HttpPost("generate")]
    public async Task<ActionResult<GenerateWorkdaysResult>> GenerateWorkdays(GenerateWorkdaysRequest request)
    {
        if (request.EndDate < request.StartDate)
            return BadRequest("EndDate must be later than StartDate");

        // find system default status
        var defaultStatus = await _context.Statuses.FirstOrDefaultAsync(s => s.Default);
        if (defaultStatus == null)
            return BadRequest("No default status configured");

        var start = request.StartDate;
        var end = request.EndDate;

        // determine department id: use request.DepartmentId, or fallback to department of first selected user
        var departmentId = request.DepartmentId;
        if (departmentId == 0 && request.SelectedUserIds != null && request.SelectedUserIds.Any())
        {
            var firstUser = await _context.Users.FindAsync(request.SelectedUserIds.First());
            if (firstUser != null)
            {
                departmentId = firstUser.DepartmentId;
            }
        }

        // load worktimes for department (may be empty)
        var workTimes = await _context.WorkTimes.Where(wt => wt.DepartmentId == departmentId).ToListAsync();

        var startDt = start.ToDateTime(TimeOnly.MinValue);
        var endDt = end.ToDateTime(TimeOnly.MaxValue);

        // load wishes for selected users that match default status and overlap the period (or are open-ended)
        var wishes = await _context.SpecialWishes
            .Where(w => w.StatusId == defaultStatus.Id && request.SelectedUserIds.Contains(w.UserId))
            .ToListAsync();

        // filter wishes by date overlap and group by dayOfWeek
        var wishesLookup = wishes
            .Where(w => (w.StartDate == null || w.StartDate <= endDt) && (w.EndDate == null || w.EndDate >= startDt))
            .GroupBy(w => w.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.Select(x => x.UserId).Distinct().ToList(), StringComparer.OrdinalIgnoreCase);

        // Load statuses to check availability for non-default wishes
        var allStatuses = await _context.Statuses.ToListAsync();

        // load non-default special wishes for selected users (we'll honor those where the status is not available)
        var nonDefaultWishes = await _context.SpecialWishes
            .Where(w => w.StatusId != defaultStatus.Id && request.SelectedUserIds.Contains(w.UserId))
            .ToListAsync();

        // filter to wishes whose status is not available and that overlap the period
        var unavailableWishes = nonDefaultWishes
            .Where(w =>
            {
                var st = allStatuses.FirstOrDefault(s => s.Id == w.StatusId);
                if (st == null) return false;
                if (st.IsAvailable) return false;
                return (w.StartDate == null || w.StartDate <= endDt) && (w.EndDate == null || w.EndDate >= startDt);
            })
            .ToList();

        // group unavailable wishes by day name for quick lookup
        var unavailableLookup = unavailableWishes
            .GroupBy(w => w.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

        // existing workdays in period (for any user) - we must not create a workday on a date that already has any workday
        var existing = await _context.Workdays
            .Where(w => w.Date >= start && w.Date <= end)
            .ToListAsync();

        // assigned counts start from existing counts but only for selected users
        var assignedCounts = request.SelectedUserIds.ToDictionary(id => id, id => existing.Count(e => e.UserId == id));

        // rotation data for Fridays
        var rotationOrder = request.SelectedUserIds.OrderBy(x => x).ToList();
        var rotationIndex = 0;

        var createdEntities = new List<Workday>();

        var rng = new Random();

        for (var d = start; d <= end; d = d.AddDays(1))
        {
            var dayName = d.DayOfWeek.ToString();
            // Determine which users are unavailable on this specific date due to non-default wishes
            var unavailableUsersForDate = new HashSet<int>();
            if (unavailableLookup.TryGetValue(dayName, out var unavailList) && unavailList != null)
            {
                foreach (var wish in unavailList)
                {
                    var wishStartOk = wish.StartDate == null || wish.StartDate <= d.ToDateTime(TimeOnly.MaxValue);
                    var wishEndOk = wish.EndDate == null || wish.EndDate >= d.ToDateTime(TimeOnly.MinValue);
                    if (!wishStartOk || !wishEndOk) continue;
                    if (!request.SelectedUserIds.Contains(wish.UserId)) continue;
                    unavailableUsersForDate.Add(wish.UserId);
                }
            }

            // if any workday exists on this date for ANY user, skip creating a new one (prevents double-assignment of the date)
            if (existing.Any(e => e.Date == d))
            {
                continue;
            }

            // FRIDAY special: assign on rotation regardless of wishes
            if (d.DayOfWeek == DayOfWeek.Friday)
            {
                if (!rotationOrder.Any()) continue;

                // find a rotation candidate who does not already have a workday on this date
                Workday? assigned = null;
                for (int i = 0; i < rotationOrder.Count; i++)
                {
                    var idx = (rotationIndex + i) % rotationOrder.Count;
                    var candidate = rotationOrder[idx];
                    // skip if candidate already has workday on this date (existing covers all users)
                    if (existing.Any(e => e.Date == d && e.UserId == candidate))
                        continue;

                    // skip if candidate has an unavailable wish for this weekday covering this date
                    if (unavailableLookup.TryGetValue(dayName, out var ulist) 
                        && ulist.Any(w => w.UserId == candidate 
                        && (w.StartDate == null || w.StartDate <= d.ToDateTime(TimeOnly.MaxValue)) 
                        && (w.EndDate == null || w.EndDate >= d.ToDateTime(TimeOnly.MinValue))))
                        continue;

                    assigned = new Workday
                    {
                        Date = d,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(17, 0),
                        UserId = candidate,
                        StatusId = defaultStatus.Id
                    };

                    rotationIndex = (idx + 1) % rotationOrder.Count;
                    break;
                }

                if (assigned == null) continue;

                // use department worktime for Friday if defined
                var fridayWork = workTimes.FirstOrDefault(wt => string.Equals(wt.DayOfWeek, "Friday", StringComparison.OrdinalIgnoreCase));
                if (fridayWork != null)
                {
                    assigned.StartTime = fridayWork.StartTime;
                    assigned.EndTime = fridayWork.EndTime;
                }

                createdEntities.Add(assigned);
                if (assignedCounts.ContainsKey(assigned.UserId)) assignedCounts[assigned.UserId]++; else assignedCounts[assigned.UserId] = 1;
                existing.Add(assigned);
                continue;
            }

            // not Friday: use wishes and balancing
            if (!wishesLookup.TryGetValue(dayName, out var candidateUserIds) || candidateUserIds == null || !candidateUserIds.Any())
            {
                continue;
            }

            // candidates who are both eligible by wish and selected, and not unavailable for this date
            var candidates = candidateUserIds.Where(id => request.SelectedUserIds.Contains(id) && !unavailableUsersForDate.Contains(id)).ToList();
            if (!candidates.Any()) continue;

            // exclude any candidate that would cause them to exceed (min + 2) after assignment
            var currentMin = assignedCounts.Values.Min();
            var allowed = candidates.Where(id => (assignedCounts.ContainsKey(id) ? assignedCounts[id] : 0) + 1 <= currentMin + 2).ToList();
            if (!allowed.Any()) allowed = candidates;

            // pick those with minimum assigned count among allowed
            var minAllowedCount = allowed.Min(id => assignedCounts.ContainsKey(id) ? assignedCounts[id] : 0);
            var pickPool2 = allowed.Where(id => (assignedCounts.ContainsKey(id) ? assignedCounts[id] : 0) == minAllowedCount).ToList();
            var chosenUser = pickPool2.Count == 1 ? pickPool2[0] : pickPool2[rng.Next(pickPool2.Count)];

            // choose start/end times from department worktimes matching this weekday
            var wt = workTimes.FirstOrDefault(x => string.Equals(x.DayOfWeek, dayName, StringComparison.OrdinalIgnoreCase));
            var startTime = wt?.StartTime ?? new TimeOnly(9,0);
            var endTime = wt?.EndTime ?? new TimeOnly(17,0);

            var workday = new Workday
            {
                Date = d,
                StartTime = startTime,
                EndTime = endTime,
                UserId = chosenUser,
                StatusId = defaultStatus.Id
            };

            createdEntities.Add(workday);
            if (assignedCounts.ContainsKey(chosenUser)) assignedCounts[chosenUser]++; else assignedCounts[chosenUser] = 1;
            existing.Add(workday);
        }

        if (createdEntities.Any())
        {
            _context.Workdays.AddRange(createdEntities);
            await _context.SaveChangesAsync();
        }

        var result = new GenerateWorkdaysResult();
        result.Created = createdEntities.Select(c => new WorkdayDTO
        {
            Date = c.Date,
            StartTime = c.StartTime,
            EndTime = c.EndTime,
            StatusId = c.StatusId,
            UserId = c.UserId
        }).ToList();

        result.AssignedCounts = assignedCounts;

        return Ok(result);
    }

    // GET: api/Workday/5
    [Authorize]
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
    [Authorize]
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
    [Authorize]
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
    [Authorize]
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
