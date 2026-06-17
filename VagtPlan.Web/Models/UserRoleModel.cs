namespace VagtPlan.Web.Models
{
    public class UserRoleModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    public class UserRoleDto
    {
        public required string Name { get; set; }
    }
}
