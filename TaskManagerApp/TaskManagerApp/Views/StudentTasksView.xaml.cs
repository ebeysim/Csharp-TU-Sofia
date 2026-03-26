using System.Windows;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Views
{
    public partial class StudentTasksView : Window
    {
        public StudentTasksView()
        {
            InitializeComponent();
            this.DataContext = new StudentViewModel();
        }
    }
}
