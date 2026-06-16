namespace ApiService.DTOS
{
    public class WorkTimeDTO
    {
        public required int DepartmentId { get; set; }
        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }
        public required string DayOfWeek { get; set; }
    }
}
