using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ClientTestApplication.Views
{
    public partial class LoginPageView : UserControl
    {
        public LoginPageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
