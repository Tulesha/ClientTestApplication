using System;
using System.IO;
using System.Threading.Tasks;

namespace ChatMessages
{
    [Serializable]
    public abstract class ImageMessage : Message
    {
        public System.Drawing.Bitmap SystemBitmap { get; set; }

        [field: NonSerialized]
        public Avalonia.Media.Imaging.Bitmap AvaloniaBitmap { get; set; }
    }
}
