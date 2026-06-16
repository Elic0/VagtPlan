using ApiService.Models;

namespace ApiService.Helpers;

public static class WorkTimeOverlapHelper
{
    public static bool Overlaps(TimeOnly startA, TimeOnly endA, TimeOnly startB, TimeOnly endB)
        => startA < endB && startB < endA;

    public static bool HasOverlap(
        IEnumerable<WorkTime> existing,
        string dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        int? excludeId = null)
    {
        return existing
            .Where(w => w.DayOfWeek == dayOfWeek && (!excludeId.HasValue || w.Id != excludeId.Value))
            .Any(w => Overlaps(startTime, endTime, w.StartTime, w.EndTime));
    }
}
