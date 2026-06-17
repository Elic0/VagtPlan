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
}
