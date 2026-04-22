using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.ViewModels
{
    public class StudentViewModel : ViewModelBase
    {
        public ObservableCollection<UserTaskAssignment> MyTasks { get; set; } = new();
        public User currentUser;
        private bool _isCompleted;
        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetProperty(ref _isCompleted, value);
        }


        private int _totalPoints = 0;

        public int TotalPoints
        {
            get => _totalPoints;
            set
            {
                SetProperty(ref _totalPoints, value);
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

        public StudentViewModel(User cU)
        {
            this.currentUser = cU;
            getMyTasks(currentUser.Id);
            TotalPoints = currentUser.TotalPoints;
        }

        public void getMyTasks(int userId)
        {
            try
            {
                using (var db = new AppDbContext())
                {

                    var tasks = db.UserTaskAssignments
                        .Where(uta => uta.UserId == userId)
                        .Include(t => t.TaskModel)
                        .OrderBy(t => t.IsCompleted)
                        .ThenByDescending(t => t.TaskModel.Priority)
                        .ThenByDescending(t => t.AssignedDate)
                        .ToList();

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MyTasks.Clear();
                        foreach (var t in tasks)
                        {
                            MyTasks.Add(t);
                        }
                    });
                }
                //MessageBox.Show($"Loaded {MyTasks.Count} tasks!");
            }
            catch (Exception ex) {
                MessageBox.Show($"Error loading tasks: {ex.Message}");
            }
        }

       
        public void MarkComplete(UserTaskAssignment taskAssignment)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    // Fetch fresh from disk
                    var assignmentInDb = db.UserTaskAssignments
                        .Include(a => a.TaskModel)
                        .FirstOrDefault(a => a.Id == taskAssignment.Id);

                    if (assignmentInDb == null) return;

                    var userInDb = db.Users.Find(currentUser.Id);
                    if (userInDb != null)
                    {
                        userInDb.TotalPoints += assignmentInDb.TaskModel.PointValue;
                        this.TotalPoints = userInDb.TotalPoints;


                        db.UserTaskAssignments.Remove(assignmentInDb);
                        db.SaveChanges();
                    }
                }
            }catch (Exception ex)
            {
                MessageBox.Show($"Error marking task complete: {ex.Message}");
                return;
            }

            getMyTasks(taskAssignment.UserId);
        }
    }
    
}