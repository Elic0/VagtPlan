using VagtPlan.Web.Models;

namespace VagtPlan.Web.Helpers;

public static class WorkTimeOverlapHelper
{
    public static bool Overlaps(TimeOnly startA, TimeOnly endA, TimeOnly startB, TimeOnly endB)
        => startA < endB && startB < endA;

    public static bool HasOverlap(
        IEnumerable<WorkTimeModel> existing,
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
