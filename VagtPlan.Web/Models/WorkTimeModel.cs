namespace VagtPlan.Web.Models;

public class WorkTimeModel
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public required string DayOfWeek { get; set; }
}

public class WorkTimeDto
{
    public int DepartmentId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public required string DayOfWeek { get; set; }
}
