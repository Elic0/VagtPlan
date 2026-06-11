using System.ComponentModel.DataAnnotations;

namespace ApiService.Models
{
    public class Common
    {
        [Key]
        public long Id { get; set; }  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}