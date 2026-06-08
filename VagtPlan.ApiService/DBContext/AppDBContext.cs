using Microsoft.EntityFrameworkCore;

namespace ApiService.DBContext
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Department> Departments { get; set; }
        public DbSet<Models.Workday> Workdays { get; set; }
        public DbSet<Models.Wish> Wishes { get; set; }
        public DbSet<Models.Substitute> Substitutes { get; set; }
        public DbSet<Models.Status> Statuses { get; set; }
        public DbSet<Models.WorkTime> WorkTimes { get; set; }
    }
}