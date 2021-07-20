using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientTestApplication.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            Content = new LoginPageViewModel();
        }

        private ViewModelBase content;
        public ViewModelBase Content
        {
            get => content;
            set => this.RaiseAndSetIfChanged(ref content, value);
        }
    }
}
