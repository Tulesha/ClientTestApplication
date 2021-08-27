using Avalonia.Media.Imaging;

namespace ClientTestApplication.Models.ChatMessages
{
    abstract class MyImage : Message 
    {
        public Bitmap Source { get; set; }
    }
}
