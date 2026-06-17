namespace VagtPlan.Web.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? password { get; set; }
        public required int VactionDays { get; set; }
        public required long DepartmentId { get; set; }
        public required long UserRoleId { get; set; }
    }

    public class UserDto
    {
        public required string Name { get; set; }
        public string? password { get; set; }
        public required int VactionDays { get; set; }
        public required long DepartmentId { get; set; }
        public required long UserRoleId { get; set; }
    }

    public class LoginDto
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
    }
}
