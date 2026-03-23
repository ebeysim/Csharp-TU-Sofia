using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Data;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Configure EF Core with SQLite
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite("Data Source=taskmanager.db"));

                    // Register view models
                    services.AddSingleton<MainWindow>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<AdminViewModel>();
                    services.AddTransient<StudentViewModel>();

                    // Register views so DI can provide view instances with injected view models
                    services.AddTransient<Views.LoginView>();
                    services.AddTransient<Views.AdminDashboardView>();
                    services.AddTransient<Views.StudentTasksView>();
                })
                .Build();

            _host.Start();

            // Ensure database is created / migrations applied
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

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
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
