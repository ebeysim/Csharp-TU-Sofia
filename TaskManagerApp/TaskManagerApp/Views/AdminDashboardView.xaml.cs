using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Views
{
    public partial class AdminDashboardView : Window
    {
        public AdminDashboardView()
        {
            InitializeComponent();
            this.DataContext = new AdminViewModel();
        }
    }
}
