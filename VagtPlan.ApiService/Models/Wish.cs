namespace ApiService.Models
{
    public class Wish : Common
    {
        public DateTime? Date { get; set; }
        public required string DayOfWeek { get; set; }
        public required int StatusId { get; set; }
        public required int UserId { get; set; }

        public Status? Status { get; set; }
        public User? User { get; set; }
    }
}
