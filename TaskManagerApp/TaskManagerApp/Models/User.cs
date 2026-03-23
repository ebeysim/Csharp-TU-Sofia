using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string? Username { get; set; }

        // Total points accumulated by the user (students)
        public int TotalPoints { get; set; }
    }
}