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

        public StudentViewModel(User currentUser)
        {
            getMyTasks(currentUser.Id);
            TotalPoints = currentUser.TotalPoints;
            //ClearAllAssignments();
        }

        public void getMyTasks(int userId)
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
                //MessageBox.Show($"Loaded {MyTasks.Count} tasks!");

            }
        }

       
        public void MarkComplete(UserTaskAssignment taskAssignment)
        {
            using (var db = new AppDbContext()) // Fresh connection
            {
                // Fetch fresh from disk
                var assignmentInDb = db.UserTaskAssignments
                    .Include(a => a.TaskModel)
                    .FirstOrDefault(a => a.Id == taskAssignment.Id);

                if (assignmentInDb == null) return;

                var userInDb = db.Users.Find(assignmentInDb.UserId);
                if (userInDb != null)
                {
                    userInDb.TotalPoints += assignmentInDb.TaskModel.PointValue;
                    this.TotalPoints = userInDb.TotalPoints;


                    db.UserTaskAssignments.Remove(assignmentInDb);
                    db.SaveChanges(); // Commits to the actual .db file
                }
            }

            getMyTasks(taskAssignment.UserId);
        }

        public void ClearAllAssignments()
        {
            using (var db = new AppDbContext())
            {
                var allAssignments = db.UserTaskAssignments.ToList();
                var allTasks = db.Tasks.ToList();
                db.UserTaskAssignments.RemoveRange(allAssignments);
                db.Tasks.RemoveRange(allTasks);

                db.SaveChanges();
            }

            App.Current.Dispatcher.Invoke(() => MyTasks.Clear());
        }
    }
    
}