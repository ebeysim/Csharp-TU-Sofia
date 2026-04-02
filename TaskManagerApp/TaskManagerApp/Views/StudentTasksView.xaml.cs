using System.Windows;
using TaskManagerApp.Models;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Views
{
    public partial class StudentTasksView : Window
    {
        public StudentTasksView(User currentUser)
        {
            InitializeComponent();
            Title.Text = $"Welcome {currentUser.Username}";
            this.DataContext = new StudentViewModel(currentUser);
            var vm = (StudentViewModel)this.DataContext;
            vm.getMyTasks(currentUser.Id);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout",
                                 MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 2. Create and show the Login window
                var loginWindow = new LoginView(); // Or whatever your login class is named
                loginWindow.Show();

                // 3. Close THIS window (This kills the reference to the current User)
                this.Close();
            }
        }
        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Delete Task Clicked");
            var vm = (StudentViewModel)this.DataContext;
            if (vm.SelectedTask == null)
            {
                MessageBox.Show("Please select a task.");
                return;
            }
            var result = MessageBox.Show($"Are you sure you want to complete the task '{vm.SelectedTask.TaskModel.Title}'?", "Complete Task",
                                 MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                vm.MarkComplete(vm.SelectedTask);
            }
        }
    }
}
