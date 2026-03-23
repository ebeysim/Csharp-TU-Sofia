using System.Windows.Controls;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Views
{
    public partial class AdminDashboardView : UserControl
    {
        public AdminDashboardView()
        {
            InitializeComponent();
        }

        public AdminDashboardView(AdminViewModel vm) : this()
        {
            DataContext = vm;
        }
    }
}
