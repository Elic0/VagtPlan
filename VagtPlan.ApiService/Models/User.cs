using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models
{
    public class User : Common
    {
        public required string Name { get; set; }
        public string? Password { get; set; }
        public required int VactionDays { get; set; }
        public required long DepartmentId { get; set; }
        public required long UserRoleId { get; set; }

        public Department? Department { get; set; }
        public UserRole? UserRole { get; set; }
    }
}
