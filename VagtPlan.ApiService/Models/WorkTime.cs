namespace ApiService.Models
{
    public class WorkTime : Common
    {
        public required int DepartmentId { get; set; }
        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }
        public required string DayOfWeek { get; set; }

        public Department? Department { get; set; }
    }
}
