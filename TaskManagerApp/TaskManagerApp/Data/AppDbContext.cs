using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Models;

namespace TaskManagerApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;

        public DbSet<TaskModel> Tasks { get; set; } = null!;

        public DbSet<UserTaskAssignment> UserTaskAssignments { get; set; } = null!;
    }
}