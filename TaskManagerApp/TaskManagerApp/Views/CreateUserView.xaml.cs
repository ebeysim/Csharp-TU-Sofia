using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskManagerApp.ViewModels;
using TaskManagerApp.Models.Enums;

namespace TaskManagerApp.Views
{
   
    public partial class CreateUserView : UserControl
    {
        public CreateUserView(){
            InitializeComponent();
        }
        // A simple event to tell the AdminDashboard to hide this control
        public event Action OnClose;

        public void Submit_Click(object sender, RoutedEventArgs e)
        {
            var vm = (AdminViewModel)this.DataContext;

            if (!string.IsNullOrWhiteSpace(UsernameBox.Text) && !string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                vm.CreateUser(UsernameBox.Text, PasswordBox.Password, UserRoles.Student);
                MessageBox.Show("Student Created!");
                OnClose?.Invoke();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => OnClose?.Invoke();
    }
}
