using System;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ChatMessages;

namespace ChatServer
{
    class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                // получаем имя пользователя
                var messageObj = GetMessage();
                var textMessage = (TextMessage)messageObj;
                userName = textMessage.Content;

                textMessage.Content = userName + " вошел в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessage(textMessage, this.Id);

                Console.WriteLine(textMessage.Content);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        messageObj = GetMessage();
                        if (messageObj as TextMessage != null)
                        {
                            textMessage = (TextMessage)messageObj;
                            textMessage.Content = String.Format("{0}: {1}", userName, textMessage.Content);
                            Console.WriteLine(textMessage.Content);
                            server.BroadcastMessage(textMessage, this.Id);
                        }
                        else if (messageObj as ImageMessage != null)
                        {
                            var imageMessage = (ImageMessage)messageObj;
                            server.BroadcastMessage(imageMessage, this.Id);
                            Console.WriteLine(imageMessage.SystemBitmap.Size);
                        }
                    }
                    catch (Exception e)
                    {
                        textMessage.Content = String.Format("{0}: покинул чат", userName);
                        Console.WriteLine(textMessage.Content);
                        server.BroadcastMessage(textMessage, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private object GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            using MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            int bytes;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                if (bytes == 0)
                    throw new Exception("Пользователь вышел");
                ms.Write(data, 0, bytes);
            }
            while (Stream.DataAvailable);

            ms.Position = 0;

            return formatter.Deserialize(ms);
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
