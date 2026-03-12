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
    }
}