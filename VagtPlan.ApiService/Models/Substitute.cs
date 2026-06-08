namespace ApiService.Models
{
    public class Substitute : Common
    {
        public required DateTime Date { get; set; }
        public string? SubstitutedName { get; set; }
        public required int SubstituteId { get; set; }

        public User? User { get; set; }
    }
}
