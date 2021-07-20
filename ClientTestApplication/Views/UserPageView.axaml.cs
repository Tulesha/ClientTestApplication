using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ClientTestApplication.Views
{
    public partial class UserPageView : UserControl
    {
        public UserPageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
