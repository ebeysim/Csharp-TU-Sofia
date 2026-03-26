using System.ComponentModel.DataAnnotations;
using TaskManagerApp.Models.Enums;

namespace TaskManagerApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } // This is now your unique login ID
        public string PasswordHash { get; set; }
        public UserRoles Role { get; set; }
        public int TotalPoints { get; set; }
    }
}