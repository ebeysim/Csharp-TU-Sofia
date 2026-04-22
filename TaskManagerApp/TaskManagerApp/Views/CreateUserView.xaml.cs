using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.ViewModels;
using TaskManagerApp.Models.Enums;

namespace TaskManagerApp.Views
{
   
    public partial class CreateUserView : UserControl
    {
        public CreateUserView(){
            InitializeComponent();
        }
        public event Action OnClose;

        public void Submit_Click(object sender, RoutedEventArgs e)
        {
            var vm = (AdminViewModel)this.DataContext;

            if (!string.IsNullOrWhiteSpace(UsernameBox.Text) && !string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                vm.CreateStudent(UsernameBox.Text, PasswordBox.Password, UserRoles.Student);
                MessageBox.Show("Student Created!");
                UsernameBox.Text = string.Empty;
                PasswordBox.Clear();
                OnClose?.Invoke();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => OnClose?.Invoke();
    }
}
