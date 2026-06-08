using Microsoft.AspNetCore.SignalR;

namespace ApiService.DTOS
{
    public class SubstituteDTO
    {
        public required DateTime Date { get; set; }
        public string? SubstitutedName { get; set; }
        public required int SubstituteId { get; set; }
    }
}
