using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Data;
using TaskManagerApp.ViewModels;
using System.IO;
using System;

namespace TaskManagerApp
{
    public partial class App : Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Get the database path consistent with AppDbContext
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbPath = Path.Join(path, "taskmanager.db");

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite($"Data Source={dbPath}"));

                    
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<AdminViewModel>();
                    services.AddTransient<StudentViewModel>();

                  
                    services.AddTransient<Views.LoginView>();
                    services.AddTransient<Views.AdminDashboardView>();
                    services.AddTransient<Views.StudentTasksView>();
                })
                .Build();

            _host.Start();

           
            try
            {
                using (var scope = _host.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.Migrate();
                }
            }
            catch
            {
                // ignore migration errors for skeleton; in real app log or handle appropriately
            }

            var loginWindow = _host.Services.GetRequiredService<Views.LoginView>();
            loginWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host is not null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }

            base.OnExit(e);
        }
    }

}
