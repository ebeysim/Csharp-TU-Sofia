using System.Windows.Controls;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Views
{
    public partial class StudentTasksView : UserControl
    {
        public StudentTasksView()
        {
            InitializeComponent();
        }

        public StudentTasksView(StudentViewModel vm) : this()
        {
            DataContext = vm;
        }
    }
}
