using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Models
{
    public class UserTaskAssignment
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TaskModelId { get; set; }

        public DateTime AssignedDate { get; set; }

        public bool IsCompleted { get; set; }

        // Navigation properties
        public User? User { get; set; }

        public TaskModel? TaskModel { get; set; }
    }
}