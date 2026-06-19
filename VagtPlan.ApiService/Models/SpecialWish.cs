namespace ApiService.Models
{
    public class SpecialWish : Common
    {
        public required DateOnly? StartDate { get; set; }
        public required DateOnly? EndDate { get; set; }
        public required string DayOfWeek { get; set; }
        public required int StatusId { get; set; }
        public required int UserId { get; set; }

        public Status? status { get; set; }
        public User? user { get; set; }
    }
}
