using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using TaskManagerApp.Models;
using TaskManagerApp.Models.Enums;

namespace TaskManagerApp.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private User _currentUser;
        public ObservableCollection<Models.User> AllUsers { get; set; } = new();
        public ObservableCollection<Models.TaskModel> AllTasks { get; set; } = new();

        public AdminViewModel(User user)
        {
            _currentUser = user;
            
        }
        
        public void CreateUser(string username, string password, UserRoles ur)
        {
            if (string.IsNullOrEmpty(username)) return;
            if (string.IsNullOrEmpty(password)) return;

            using (var db = new Data.AppDbContext())
            {
                if (db.Users.Any(u => u.Username == username))
                {
                    return;
                }
                var newUser = new Models.User
                {
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = ur
                };
                db.Users.Add(newUser);
                db.SaveChanges();
            }

        }

        public void DeleteUser(int userId)
        {
            using (var db = new Data.AppDbContext())
            {
                var user = db.Users.Find(userId);
                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
            }


        }

        public void CreateTask(string title, string description, Priority priority, int pointValue)
        {
            if (string.IsNullOrEmpty(title)) return;
            if (pointValue < 0) return;
            using (var db = new Data.AppDbContext())
            {
                var newTask = new Models.TaskModel
                {
                    Title = title,
                    Description = description,
                    Priority = priority,
                    PointValue = pointValue
                };
                db.Tasks.Add(newTask);
                db.SaveChanges();
            }
        }

        public void DeleteTask(int taskId)
        {
            using (var db = new Data.AppDbContext())
            {
                var task = db.Tasks.Find(taskId);
                if (task != null)
                {
                    db.Tasks.Remove(task);
                    db.SaveChanges();
                }
            }
        }

        public void EditTask(int taskId, string title, string description, Priority priority)
        {
            if (string.IsNullOrEmpty(title)) return;
            int pointValue = 0;
            switch (priority)
            {
                case Priority.Low:
                    pointValue += 1; break;
                case Priority.Medium:
                    pointValue += 2; break;
                case Priority.High:
                    pointValue += 3; break;
                case Priority.Critical:
                    pointValue += 5;
                    break;
                default:
                    return;
            }
            using (var db = new Data.AppDbContext())
            {
                var task = db.Tasks.Find(taskId);
                if (task != null)
                {
                    task.Title = title;
                    task.Description = description;
                    task.Priority = priority;
                    task.PointValue = pointValue;
                    db.SaveChanges();
                }
            }
        }
        public void AssignTaskToUser(int taskId, int userId)
        {
            using (var db = new Data.AppDbContext())
            {
                var task = db.Tasks.Find(taskId);
                var user = db.Users.Find(userId);
                if (task != null && user != null)
                {
                    var newAssignment = new Models.UserTaskAssignment
                    {
                        UserId = userId,
                        TaskId = taskId,
                        AssignedDate = DateTime.Now,
                        IsCompleted = false,
                        User = user,
                        TaskModel = task
                    };
                }
            }
        }
    }
}