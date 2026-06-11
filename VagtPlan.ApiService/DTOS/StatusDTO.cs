namespace ApiService.DTOS
{
    public class StatusDTO
    {
        public required string Name { get; set; }
        public required string Colour { get; set; }
        public required bool IsAvailable { get; set; }
        public required bool Default { get; set; }
    }
}
