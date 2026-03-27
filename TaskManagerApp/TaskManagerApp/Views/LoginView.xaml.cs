using System.Windows;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = (LoginViewModel)this.DataContext;
                viewModel.ExecuteLogin(PasswordInput.Password);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
