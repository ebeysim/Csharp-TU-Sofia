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
using TaskManagerApp.Models;
using TaskManagerApp.Models.Enums;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Views
{
    
    public partial class DeleteUsersView : UserControl
    {
        public DeleteUsersView()
        {
            InitializeComponent();

        }
        public event Action OnClose;

        public void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            var vm = (AdminViewModel)this.DataContext;
            var selectedStudents = UsersDeleteList.SelectedItems.Cast<User>().ToList();

            if (selectedStudents.Count == 0)
            {
                MessageBox.Show("Please select at least one user to delete.", "No Selection",
                                 MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout",
                                 MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {

                foreach (var student in selectedStudents)
                {
                    vm.DeleteUser(student.Id);
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => OnClose?.Invoke();
    }
}
