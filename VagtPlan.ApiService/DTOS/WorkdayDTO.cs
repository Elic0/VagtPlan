namespace ApiService.DTOS
{
    public class WorkdayDTO
    {
        public required DateOnly Date { get; set; }
        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }
        public required int StatusId { get; set; }
        public required int UserId { get; set; }
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
        public Dictionary<int, int> AssignedCounts { get; set; } = new();
    }
}
