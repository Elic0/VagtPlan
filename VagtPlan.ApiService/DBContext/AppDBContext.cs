using Microsoft.EntityFrameworkCore;
using ApiService.Models;

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
        public DbSet<Models.UserRole> UserRoles { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Common>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().ToTable("UserRole");
        }
    }
}