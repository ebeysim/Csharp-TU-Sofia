using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.Models.Enums;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Views
{
    public partial class AdminDashboardView : Window
    {

        public AdminDashboardView()
        {
            InitializeComponent();
            this.DataContext = new AdminViewModel();
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
    }
}
