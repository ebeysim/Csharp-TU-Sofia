using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.Models;
using TaskManagerApp.Models.Enums;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Views
{
    public partial class AdminDashboardView : Window
    {

        public AdminDashboardView(User user)
        {
            InitializeComponent();
           
            Title.Text = $"Welcome {user.Username}";
            this.DataContext = new AdminViewModel(user);
            
            CreateUserPopup.OnClose += () =>
            {
                UserOverlay.Visibility = Visibility.Collapsed;

            };
            CreateTaskPopup.OnClose += () =>
            {
                TaskOverlay.Visibility = Visibility.Collapsed;

            };
        }
        private void OpenCreateUser_Click(object sender, RoutedEventArgs e)
        {
            UserOverlay.Visibility = Visibility.Visible;

        }
        private void OpenCreateTask_Click(object sender, RoutedEventArgs e)
        {
            TaskOverlay.Visibility = Visibility.Visible;
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
    }
}
