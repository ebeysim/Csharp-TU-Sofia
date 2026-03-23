namespace TaskManagerApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string? _username;
        public string? Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public RelayCommand? LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => { /* login logic placeholder */ });
        }
    }
}