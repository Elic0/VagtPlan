namespace ApiService.DTOS
{
    public class SpecialWishDTO
    {
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public required string DayOfWeek { get; set; }
        public required int StatusId { get; set; }
        public required int UserId { get; set; }
    }
}
