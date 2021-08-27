using Avalonia.Layout;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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

        public delegate void PrintMessageHandler(string message, HorizontalAlignment side);
        public delegate void PrintImageHandler(byte[] imageData, HorizontalAlignment side);
        public event PrintMessageHandler PrintMessage;
        public event PrintImageHandler PrintImage;

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

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
                
                receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.IsBackground = true;
                receiveThread.Start();

                // Добро пожаловать 
                PrintMessage("Добро пожаловать ", HorizontalAlignment.Left);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);

            PrintMessage($"Вы: {message}", HorizontalAlignment.Right);
        }

        public void SendImage(byte[] data)
        {
            stream.Write(data, 0, data.Length);

            PrintImage(data, HorizontalAlignment.Right);
        }

        private void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string receiveMessage = builder.ToString();

                    if (receiveMessage.Length < 1000)
                    {
                        PrintMessage(receiveMessage, HorizontalAlignment.Left);
                    }
                    else
                    {
                        PrintImage(Encoding.Unicode.GetBytes(receiveMessage), HorizontalAlignment.Left);
                    }
                }
                catch
                {
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
