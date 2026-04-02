using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.ViewModels
{
    public class StudentViewModel : ViewModelBase
    {
        //private User _currentUser;
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

       

        //public void MarkComplete(UserTaskAssignment taskAssignment)
        //{
        //    if (taskAssignment == null) return;

        //    using (var db = new AppDbContext())
        //    {
        //        // FIX 1: Use Include() so TaskModel (and its PointValue) is actually loaded
        //        var assignmentInDb = db.UserTaskAssignments
        //            .Include(a => a.TaskModel)
        //            .FirstOrDefault(a => a.Id == taskAssignment.Id);

        //        if (assignmentInDb == null || assignmentInDb.TaskModel == null)
        //        {
        //            MessageBox.Show("Task data could not be loaded.");
        //            return;
        //        }

        //        var userInDb = db.Users.Find(assignmentInDb.UserId);

        //        if (userInDb != null)
        //        {
        //            // Update the Database values
        //            assignmentInDb.IsCompleted = true;
        //            userInDb.TotalPoints += taskAssignment.TaskModel.PointValue;

        //            try
        //            {
        //                db.SaveChanges();

        //                // Show the correct points in the message
        //                MessageBox.Show($"Earned {taskAssignment.TaskModel.PointValue} points! Total: {userInDb.TotalPoints}");
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"Error: {ex.Message}");
        //                return;
        //            }

        //            // FIX 2: Update the UI
        //            App.Current.Dispatcher.Invoke(() =>
        //            {
        //                // Update the ViewModel's top-level points property
        //                this.TotalPoints = userInDb.TotalPoints;

        //                MyTasks.FirstOrDefault(t => t.Id == taskAssignment.Id)?.IsCompleted = true;

        //                SelectedTask = null;
        //            });
        //        }
        //    }
        //}
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

                    //Delete the link
                    db.UserTaskAssignments.Remove(assignmentInDb);
                    db.SaveChanges(); // Commits to the actual .db file
                }
            }
            // Now reload the list from the disk
            getMyTasks(taskAssignment.UserId);
        }

        public void ClearAllAssignments()
        {
            using (var db = new AppDbContext())
            {
                // This tells EF to track every single row for deletion
                var allAssignments = db.UserTaskAssignments.ToList();
                var allTasks = db.Tasks.ToList();
                db.UserTaskAssignments.RemoveRange(allAssignments);
                db.Tasks.RemoveRange(allTasks);

                db.SaveChanges();
            }

            // Refresh the UI so the list goes empty
            App.Current.Dispatcher.Invoke(() => MyTasks.Clear());
        }
    }
    
}