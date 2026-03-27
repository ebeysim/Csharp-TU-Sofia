using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using TaskManagerApp.Data;
using TaskManagerApp.Models;
using TaskManagerApp.Models.Enums;
using TaskManagerApp.Views;

namespace TaskManagerApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string? _username;
        private string? _errMessage;
        public string? Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string? ErrorMessage
        {
            get => _errMessage;
            set => SetProperty(ref _errMessage, value);
        }

        // 1. Remove the 'Password' property from here. 
        // We will get it directly from the View for security.

        public void ExecuteLogin(string clearTextPassword)
        {
            if (string.IsNullOrEmpty(this.Username)) return;

            using (var db = new AppDbContext())
            {
                // Search for the user
                var user = db.Users.FirstOrDefault(u => u.Username == this.Username);

                if (user != null && BCrypt.Net.BCrypt.Verify(clearTextPassword, user.PasswordHash))
                {
                    // Open the appropriate dashboard
                    if (user.Role == UserRoles.Admin)
                    {
                        var adminDashboard = new AdminDashboardView(user);
                        adminDashboard.Show();
                    }
                    else
                    {
                        var userDashboard = new StudentTasksView(user);
                        userDashboard.Show();
                    }


                    // Close the login window
                    System.Windows.Application.Current.Windows
                        .OfType<LoginView>()
                        .FirstOrDefault()
                        ?.Close();
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            }
        }
    }
}