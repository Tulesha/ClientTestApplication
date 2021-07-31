using ReactiveUI;

namespace ClientTestApplication.ViewModels
{
    class UserPageViewModel : ViewModelBase
    {
        public UserPageViewModel(string userName)
        {
            ChatUserControl = new ClientChatViewModel(userName);
            MainPageControl = new MainPageViewModel(userName);
            TodoListControl = new TODOListViewModel(userName);
        }

        private ViewModelBase chatUserControl;
        private ViewModelBase mainPageControl;
        private ViewModelBase todoListControl;

        public ViewModelBase ChatUserControl
        {
            get => chatUserControl;
            private set => this.RaiseAndSetIfChanged(ref chatUserControl, value);
        }

        public ViewModelBase MainPageControl
        {
            get => mainPageControl;
            private set => this.RaiseAndSetIfChanged(ref mainPageControl, value);
        }
        
        public ViewModelBase TodoListControl
        {
            get => todoListControl;
            private set => this.RaiseAndSetIfChanged(ref todoListControl, value);
        }
    }
}
