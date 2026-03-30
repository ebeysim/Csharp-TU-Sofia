using System.ComponentModel.DataAnnotations;
using TaskManagerApp.Models.Enums;

namespace TaskManagerApp.Models
{
    public class TaskModel
    {
        [Key]
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public Priority Priority { get; set; }

        // Points awarded when task is completed
        public int PointValue { get; set; }
    }
}