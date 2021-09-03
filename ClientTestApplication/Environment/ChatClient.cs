using Avalonia.Layout;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ChatMessages;

namespace ClientTestApplication.Environments
{
    class ChatClient
    {
        private string userName;
        private string host;
        private int port;
        private TcpClient client;
        Thread receiveThread;
        private NetworkStream stream;

        public delegate void PrintTextMessageHandler(TextMessage message);
        public delegate void PrintImageMessageHandler(ImageMessage image);
        public event PrintTextMessageHandler PrintTextMessage;
        public event PrintImageMessageHandler PrintImageMessage;

        public ChatClient(string name, string host="127.0.0.1", int port=8888)
        {
            userName = name;
            this.host = host;
            this.port = port;
        }

        public void Start()
        {
            client = new TcpClient();
            try
            {
                client.Connect(host, port);
                stream = client.GetStream();

                using MemoryStream ms = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();

                var message = new IncommingTextMessage { Content = userName };
                formatter.Serialize(ms, message);
                byte[] data = ms.ToArray();
                stream.Write(data, 0, data.Length);
                
                receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.IsBackground = true;
                receiveThread.Start();

                // Добро пожаловать 
                PrintTextMessage(new IncommingTextMessage { Content = "Добро пожаловать" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void SendMessage(TextMessage message)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new IncommingTextMessage { Content = message.Content });

            byte[] data = ms.ToArray();
            stream.Write(data, 0, data.Length);

            message.Content = $"Вы: {message.Content}";
            PrintTextMessage(message);
        }

        public void SendImage(ImageMessage image)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using MemoryStream ms = new MemoryStream();

            try
            {
                formatter.Serialize(ms, new IncommingImageMessage { SystemBitmap = image.SystemBitmap });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            byte[] data = ms.ToArray();
            stream.Write(data, 0, data.Length);

            PrintImageMessage(image);
        }

        private void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];
                    using MemoryStream ms = new MemoryStream();
                    BinaryFormatter formatter = new BinaryFormatter();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        ms.Write(data, 0, bytes);
                    }
                    while (stream.DataAvailable);

                    ms.Position = 0;

                    var obj = formatter.Deserialize(ms);
                    if (obj as TextMessage != null)
                    {
                        PrintTextMessage((IncommingTextMessage)obj);
                    }
                    else if (obj as ImageMessage != null)
                    {
                        var image = (IncommingImageMessage)obj;
                        PrintImageMessage(image);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Подключение прервано");
                    Disconnect();
                    break;
                }
            }
        }

        public void Disconnect()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
