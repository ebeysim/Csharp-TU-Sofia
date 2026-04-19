using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Data;
using TaskManagerApp.Data;
using TaskManagerApp.Models;
using TaskManagerApp.Models.Enums;
using TaskManagerApp.Views;

namespace TaskManagerApp.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        public ObservableCollection<User> AllStudents { get; set; } = new();
        public ObservableCollection<UserTaskAssignment> AllTasks { get; set; } = new();
        public ObservableCollection<User> TopUsers { get; set; } = new();

        public ICollectionView TaskView { get; set; }


        private string _selectedFilter = "All";
        public string SelectedFilter
        {
            get => _selectedFilter;
            set{

                if (SetProperty(ref _selectedFilter, value))
                {
                    // IMPORTANT: Refresh the filter when the dropdown changes
                    TaskView?.Refresh();
                }
            }
        }

        private UserTaskAssignment _selectedTask;
        public UserTaskAssignment SelectedTask
        {
            get => _selectedTask;
            set
            {
                SetProperty(ref _selectedTask, value);
            }
        }

        public AdminViewModel(User currentUser)
        {
            TaskView = CollectionViewSource.GetDefaultView(AllTasks);
            InitializeFiltering();
            getTasks();
            getStudents();
            LoadTopUsers();
            
        }
        private void InitializeFiltering()
        {
            if (TaskView == null) return;

            TaskView.Filter = (obj) =>
            {
                // If "All" is selected, don't hide anything
                if (string.IsNullOrEmpty(SelectedFilter) || SelectedFilter == "All")
                    return true;

                if (obj is UserTaskAssignment item)
                {
                    // Convert the UI string back to your Enum for a type-safe check
                    if (Enum.TryParse(SelectedFilter, out Priority filterEnum))
                    {
                        return item.TaskModel.Priority == filterEnum;
                    }
                }
                return false;
            };
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
                var newUser = new User
                {
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = ur
                };
                db.Users.Add(newUser);
                db.SaveChanges();
                AllStudents.Add(newUser);
            }

        }
        public void getStudents()
        {
            using (var db = new AppDbContext())
            {
                var students = db.Users.Where(u => u.Role == UserRoles.Student).ToList();

                App.Current.Dispatcher.Invoke(() =>
                {
                    AllStudents.Clear();
                    foreach (var s in students)
                    {
                        AllStudents.Add(s);
                    }
                });
            }
            //MessageBox.Show($"Loaded {AllStudents.Count} students!");
        }
        public void getTasks()
        {
            using (var db = new AppDbContext())
            {
                var tasks = db.UserTaskAssignments
                    .Include(t => t.User)
                    .Include(t => t.TaskModel)
                    .AsNoTracking()
                    .ToList();

                App.Current.Dispatcher.Invoke(() =>
                {
                    AllTasks.Clear();
                    foreach (var t in tasks)
                    {
                        AllTasks.Add(t);
                    }
                });
                //MessageBox.Show($"Loaded {AllTasks.Count} tasks!");

            }
        }

        public void DeleteUser(int userId)
        {
            using (var db = new Data.AppDbContext())
            {
                var user = db.Users.Find(userId);
                if (user != null)
                {
                    // 1. Remove assignments first to avoid Foreign Key errors
                    var assignments = db.UserTaskAssignments.Where(a => a.UserId == userId);
                    db.UserTaskAssignments.RemoveRange(assignments);

                    // 2. Remove the user
                    db.Users.Remove(user);
                    db.SaveChanges();

                    // 3. Update the UI Collection correctly
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        // Find the exact object instance currently in the list by its ID
                        var userInList = AllStudents.FirstOrDefault(u => u.Id == userId);
                        if (userInList != null)
                        {
                            AllStudents.Remove(userInList);
                        }
                    });
                }
            }
        }

        public void CreateTask(Models.Task newTask)
        {
            var pointValue = 0;
            switch (newTask.Priority)
            {
                case Priority.Low:
                    pointValue = 1; break;
                case Priority.Medium:
                    pointValue = 2; break;
                case Priority.High:
                    pointValue = 3; break;
                case Priority.Critical:
                    pointValue = 5;
                    break;
                default:
                    return;
            }
            newTask.PointValue = pointValue;
            using (var db = new Data.AppDbContext())
            {
                db.Tasks.Add(newTask);
                db.SaveChanges();
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
                    var newAssignment = new UserTaskAssignment
                    {
                        UserId = userId,
                        TaskId = taskId,
                        AssignedDate = DateTime.Now,
                        IsCompleted = false,
                        User = db.Users.Find(userId),
                        TaskModel = db.Tasks.Find(taskId)
                    };
                    db.UserTaskAssignments.Add(newAssignment);
                    db.SaveChanges();
                    AllTasks.Add(newAssignment);
                }
            }
        }

        public void DeleteTask(int taskId, int studentId)
        {
            using (var db = new Data.AppDbContext())
            {
                var assignment = db.UserTaskAssignments.FirstOrDefault(t => t.TaskId == taskId && t.UserId == studentId);
                db.UserTaskAssignments.Remove(assignment);

                bool isOnlyAssignment = db.UserTaskAssignments.Any(t => t.TaskId == taskId && t.UserId != studentId);
                if (!isOnlyAssignment)
                {
                    var taskToRemove = db.Tasks.Find(taskId);
                    if(taskToRemove != null) db.Tasks.Remove(taskToRemove);

                }
                db.SaveChanges();
            }

            // 4. Update the UI Collection (Thread-Safe)
            App.Current.Dispatcher.Invoke(() =>
            {
                var itemInList = AllTasks.FirstOrDefault(t => t.TaskId == taskId && t.UserId == studentId);

                if (itemInList != null)
                {
                    AllTasks.Remove(itemInList);
                }
                SelectedTask = null;
            });
        }

        public void UpdateTaskView(int id)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var itemInList = AllTasks.FirstOrDefault(t => t.Id == id);
                if (itemInList != null)
                {
                    AllTasks.Remove(itemInList);
                }
                SelectedTask = null;
            });
        }

        public void DeleteTask(int assignmentId)
        {
            using (var db = new Data.AppDbContext())
            {
                var assignment = db.UserTaskAssignments
                    .Include(a => a.TaskModel)
                    .FirstOrDefault(a => a.Id == assignmentId);

                if (assignment == null) return;

                int taskId = assignment.TaskId;

                db.UserTaskAssignments.Remove(assignment);

                bool otherStudentsHaveThisTask = db.UserTaskAssignments
                    .Any(t => t.TaskId == taskId && t.Id != assignmentId);

                if (!otherStudentsHaveThisTask)
                {
                    var taskTemplate = db.Tasks.Find(taskId);
                    if (taskTemplate != null)
                    {
                        db.Tasks.Remove(taskTemplate);
                    }
                }
                db.SaveChanges();
            }
            UpdateTaskView(assignmentId);
        }

      

        public void ApproveTask(int assignmentId)
        {
            using (var db = new Data.AppDbContext())
            {
                // 1. Fetch the assignment with the Task details for points
                var assignment = db.UserTaskAssignments
                    .Include(a => a.TaskModel)
                    .FirstOrDefault(t => t.Id == assignmentId);

                if (assignment == null) return;

                var user = db.Users.Find(assignment.UserId);
                if (user != null)
                {
                    user.TotalPoints += assignment.TaskModel.PointValue;
                }
                db.UserTaskAssignments.Remove(assignment);
                db.SaveChanges();

               UpdateTaskView(assignmentId);
                LoadTopUsers();
            }
        }

        public void LoadTopUsers()
        {
            using (var db = new Data.AppDbContext())
            {
                var topUsers = db.Users
                    .Where(u => u.Role == UserRoles.Student)
                    .OrderByDescending(u => u.TotalPoints)
                    .Take(5)
                    .ToList();

                App.Current.Dispatcher.Invoke(() =>
                {
                    TopUsers.Clear();
                    foreach (var u in topUsers)
                    {
                        TopUsers.Add(u);
                    }
                });
            }
        }
    }
}