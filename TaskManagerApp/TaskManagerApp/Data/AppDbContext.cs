using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Models;
using System;
using System.IO;
using TaskManagerApp.Models.Enums;

namespace TaskManagerApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Models.Task> Tasks { get; set; } = null!;
        public DbSet<UserTaskAssignment> UserTaskAssignments { get; set; } = null!;

        public string DbPath { get; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "taskmanager.db");
        }

        public AppDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "taskmanager.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            
            if (!options.IsConfigured)
            {
                options.UseSqlite($"Data Source={DbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTaskAssignment>()
                .HasKey(ua => new { ua.UserId, ua.TaskId });

        
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                Role = UserRoles.Admin,
                TotalPoints = 0
            });
        }
    }
}