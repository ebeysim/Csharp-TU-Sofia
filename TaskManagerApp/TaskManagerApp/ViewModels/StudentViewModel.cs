namespace TaskManagerApp.ViewModels
{
    public class StudentViewModel : ViewModelBase
    {
        private string? _title = "Student Tasks";
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public StudentViewModel()
        {
        }
    }
}