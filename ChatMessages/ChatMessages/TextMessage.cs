using System;

namespace ChatMessages
{
    [Serializable]
    public abstract class TextMessage : Message
    {
        public string Content { get; set; }
    }
}
