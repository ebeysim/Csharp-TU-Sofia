using System.Windows.Controls;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public LoginView(LoginViewModel vm) : this()
        {
            DataContext = vm;
        }
    }
}
