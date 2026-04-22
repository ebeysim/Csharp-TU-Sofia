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



        public void ExecuteLogin(string clearTextPassword)
        {
            if (string.IsNullOrEmpty(this.Username)) return;

            using (var db = new AppDbContext())
            {
               
                var user = db.Users.FirstOrDefault(u => u.Username == this.Username);

                if (user != null && BCrypt.Net.BCrypt.Verify(clearTextPassword, user.PasswordHash))
                {
                    
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