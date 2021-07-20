using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ClientTestApplication.Views
{
    public partial class TODOListView : UserControl
    {
        public TODOListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
