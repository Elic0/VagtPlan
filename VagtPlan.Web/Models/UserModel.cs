namespace VagtPlan.Web.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? password { get; set; }
        public required int VactionDays { get; set; }
        public required int DepartmentId { get; set; }
        public required int UserRoleId { get; set; }
    }

    public class UserDto
    {
        public required string Name { get; set; }
        public string? password { get; set; }
        public required int VactionDays { get; set; }
        public required int DepartmentId { get; set; }
        public required int UserRoleId { get; set; }
    }

    public class LoginDto
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
    }
    public class UserRequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
    }
}
