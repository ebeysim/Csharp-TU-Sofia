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
               
                if (string.IsNullOrEmpty(SelectedFilter) || SelectedFilter == "All")
                    return true;

                if (obj is UserTaskAssignment item)
                {
                    
                    if (Enum.TryParse(SelectedFilter, out Priority filterEnum))
                    {
                        return item.TaskModel.Priority == filterEnum;
                    }
                }
                return false;
            };
        }
        public void CreateStudent(string username, string password, UserRoles ur)
        {
            if (string.IsNullOrEmpty(username)) return;
            if (string.IsNullOrEmpty(password)) return;

            try
            {
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
            }catch (Exception ex) {
                MessageBox.Show($"Error creating user: {ex.Message}"); return;
            }
        }
        public void getStudents()
        {
            try
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
            }catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}");
                return;
            }
            //MessageBox.Show($"Loaded {AllStudents.Count} students!");
        }

        //public async System.Threading.Tasks.Task getStudentsAsync()
        //{
        //    using (var db = new AppDbContext())
        //    {
        //        var students = await db.Users.Where(u => u.Role == UserRoles.Student).ToListAsync();
        //        AllStudents.Clear();
        //        foreach (var s in students)
        //        {
        //            AllStudents.Add(s);
        //        }
        //    }
        //}
        public void getTasks()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}");
                return;
            }
        }

        public void DeleteUser(int userId)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}");
                return;
            }
            TopUsers.Clear();
            LoadTopUsers();
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
            try
            {
                using (var db = new Data.AppDbContext())
                {
                    db.Tasks.Add(newTask);
                    db.SaveChanges();
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error creating task: {ex.Message}");
                return;
            }
                                
        }



        public void AssignTaskToUser(int taskId, int userId)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error assigning task: {ex.Message}");
                return;
            }
        }

        public void DeleteTask(int taskId, int studentId)
        {
            try
           {
                using (var db = new Data.AppDbContext())
                {
                    var assignment = db.UserTaskAssignments.FirstOrDefault(t => t.TaskId == taskId && t.UserId == studentId);
                    db.UserTaskAssignments.Remove(assignment);

                    bool isOnlyAssignment = db.UserTaskAssignments.Any(t => t.TaskId == taskId && t.UserId != studentId);
                    if (!isOnlyAssignment)
                    {
                        var taskToRemove = db.Tasks.Find(taskId);
                        if (taskToRemove != null) db.Tasks.Remove(taskToRemove);

                    }
                    db.SaveChanges();
                }
            }catch (Exception ex)
            {
                MessageBox.Show($"Error deleting task: {ex.Message}");
                return;
            }
            getTasks();
        }




        public void LoadTopUsers()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading top users: {ex.Message}");
                
            }
        }

        public void ApproveTask(int taskId, int studentId)
        {
            try
            {
                using (var db = new Data.AppDbContext())
                {
                    var assignment = db.UserTaskAssignments
                        .Include(a => a.TaskModel)
                        .FirstOrDefault(t => t.TaskId == taskId && t.UserId == studentId);

                    if (assignment == null) return;

                    var user = db.Users.Find(assignment.UserId);
                    if (user != null)
                    {
                        user.TotalPoints += assignment.TaskModel.PointValue;
                    }
                    DeleteTask(taskId, studentId);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error approving task: {ex.Message}");
                return;
            }
            getTasks();
            LoadTopUsers();
        }

        
    }
}