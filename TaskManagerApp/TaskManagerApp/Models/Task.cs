using System.ComponentModel.DataAnnotations;
using TaskManagerApp.Models.Enums;

namespace TaskManagerApp.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public Priority Priority { get; set; }

        public int PointValue { get; set; }
    }
}