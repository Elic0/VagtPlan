namespace VagtPlan.Web.Models;

public class DepartmentModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class DepartmentDto
{
    public required string Name { get; set; }
}
