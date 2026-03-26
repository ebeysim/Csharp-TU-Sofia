namespace TaskManagerApp.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private string? _title = "Admin Dashboard";
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public AdminViewModel()
        {

        }
    }
}