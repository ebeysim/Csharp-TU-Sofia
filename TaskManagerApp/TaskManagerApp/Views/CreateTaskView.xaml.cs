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
using TaskManagerApp.Models.Enums;
using TaskManagerApp.Models;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Views
{
    /// <summary>
    /// Interaction logic for CreateTaskView.xaml
    /// </summary>
    public partial class CreateTaskView : UserControl
    {
        public CreateTaskView()
        {
            InitializeComponent();
        }
        public event Action OnClose;

        private void SaveTask_Click(object sender, RoutedEventArgs e)
        {
            
            // 1. Get the list of Users that have their checkbox checked
            // We cast them to 'User' because that is what's inside the AllUsers collection
            var selectedStudents = UserSelectionList.SelectedItems.Cast<User>().ToList();

            if (selectedStudents.Count == 0)
            {
                MessageBox.Show("Please select at least one student to assign this task to.");
                return;
            }

            // 2. Determine Points (based on your priority logic)
            string priorityStr = ((ComboBoxItem)PriorityCombo.SelectedItem).Content.ToString();
            Priority selectedPriority = (Priority)Enum.Parse(typeof(Priority), priorityStr);
            var newTask = new Models.Task
            {
                Title = TitleBox.Text,
                Description = DescBox.Text,
                Priority = selectedPriority,
            };
            // 3. Send everything to the ViewModel to save to DB
            var vm = (AdminViewModel)this.DataContext;
            vm.CreateTask(newTask);

            foreach (var student in selectedStudents)
            {
                vm.AssignTaskToUser(newTask.Id, student.Id);
            }
            TitleBox.Text = String.Empty;
            DescBox.Text = String.Empty;
            // 4. Close the popup
            OnClose?.Invoke();
        }
        

        private void Cancel_Click(object sender, RoutedEventArgs e) => OnClose?.Invoke();
    }
}
