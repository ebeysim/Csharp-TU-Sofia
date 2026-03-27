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
        }
    }
}
