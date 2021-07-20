using ReactiveUI;

namespace ClientTestApplication.ViewModels
{
    class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(string userName)
        {
            UserName = userName;
        }

        private string userName;
        public string UserName
        {
            get => userName;
            private set => this.RaiseAndSetIfChanged(ref userName, value);
        }
    }

}
