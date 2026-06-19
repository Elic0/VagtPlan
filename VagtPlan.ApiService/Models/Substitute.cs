namespace ApiService.Models
{
    public class Substitute : Common
    {
        public required DateTime Date { get; set; }
        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }
        public string? SubstitutedName { get; set; }
        public required int SubstituteId { get; set; }
        public required int UserId { get; set; }

        public User? User { get; set; }
    }
}
