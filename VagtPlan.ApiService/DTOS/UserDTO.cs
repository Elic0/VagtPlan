namespace ApiService.DTOS
{
    public class UserDTO
    {
        public required string Name { get; set; }
        public required int VactionDays { get; set; }
        public required long DepartmentId { get; set; }
        public required long UserRoleId { get; set; }
    }

    public class LoginDTO
    {
        public required string Name { get; set; }
        public required string Password { get; set; }

    }
}
