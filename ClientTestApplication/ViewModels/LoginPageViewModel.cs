using Avalonia.Controls;
using Avalonia.Input;
using ReactiveUI;
using System.Windows.Input;

namespace ClientTestApplication.ViewModels
{
    class LoginPageViewModel : ViewModelBase
    {
        public LoginPageViewModel()
        {
            var enable = this.WhenAnyValue(
                x => x.UserName,
                (userName) => !string.IsNullOrWhiteSpace(userName)
            );

            LogInCommand = ReactiveCommand.Create((Window window) =>
            {
                window.Content = new UserPageViewModel(UserName);
            }, enable);
        }

        private string userName;

        public string UserName
        {
            get => userName;
            private set => this.RaiseAndSetIfChanged(ref userName, value);
        }

        public ICommand LogInCommand { get; }
    }
}
