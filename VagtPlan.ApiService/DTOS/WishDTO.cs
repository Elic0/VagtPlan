namespace ApiService.DTOS
{
    public class WishDTO
    {
        public required string DayOfWeek { get; set; }
        public required int StatusId { get; set; }
        public required int UserId { get; set; }
    }
}
