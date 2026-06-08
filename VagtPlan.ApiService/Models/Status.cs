namespace ApiService.Models
{
    public class Status : Common
    {
        public required string Name { get; set; }
        public required string Color { get; set; }
        public required bool IsAvailable { get; set; }
        public required bool Default { get; set; }
    }
}
