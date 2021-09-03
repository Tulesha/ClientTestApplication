using Avalonia.Layout;
using Avalonia.Controls;
using ClientTestApplication.Environments;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ChatMessages;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;

namespace ClientTestApplication.ViewModels
{
    class ClientChatViewModel : ViewModelBase
    {
        public ClientChatViewModel(string userName)
        {
            Messages = new ObservableCollection<Message>();


            ChatClient chatClient = new ChatClient(userName);
            chatClient.PrintTextMessage += ChatClient_PrintTextMessage;
            chatClient.PrintImageMessage += ChatClient_PrintImageMessage;

            ConnectCommand = ReactiveCommand.Create(() =>
            {
                if (ConnectDisconnect == "Подключиться")
                {
                    EnableChat = false;
                    ConnectDisconnect = "Отключиться";

                    chatClient.Start();
                }
                else
                {
                    EnableChat = true;
                    InputMessage = string.Empty;
                    ConnectDisconnect = "Подключиться";

                    chatClient.Disconnect();
                }
            });

            SendMessageCommand = ReactiveCommand.Create(() =>
            {
                if (InputMessage != string.Empty)
                {
                    chatClient.SendMessage(new OutGoingTextMessage { Content = InputMessage });
                    InputMessage = string.Empty;
                }
            });

            PickPictureCommand = ReactiveCommand.Create(async (Window window) =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filters.Add(new FileDialogFilter() { Name = "Выберите фото", Extensions = { "jpg" } });
                string[] result = await dialog.ShowAsync(window);


                if (result.Length != 0)
                {
                    FileName = string.Join(" ", result);

                    MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(FileName));
                    chatClient.SendImage(new OutGoingImageMessage { SystemBitmap = new Bitmap(memoryStream) });
                }
            });
        }

        private void ChatClient_PrintTextMessage(TextMessage message)
        {
            Messages.Add(message);
        }

        private async void ChatClient_PrintImageMessage(ImageMessage image)
        {
            using MemoryStream ms = new MemoryStream();
            image.SystemBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            ms.Position = 0;

            image.AvaloniaBitmap = await Task.Run(() => Avalonia.Media.Imaging.Bitmap.DecodeToWidth(ms, 400));
            Messages.Add(image);
        }


        private string inputMessage = string.Empty;
        private string connectDisconnect = "Подключиться";
        private bool enableChat = true;

        public ObservableCollection<Message> Messages { get; set; }

        public string InputMessage
        {
            get => inputMessage;
            private set => this.RaiseAndSetIfChanged(ref inputMessage, value);
        }
        public bool EnableChat
        {
            get => enableChat;
            set => this.RaiseAndSetIfChanged(ref enableChat, value);
        }
        public string ConnectDisconnect
        {
            get => connectDisconnect;
            set => this.RaiseAndSetIfChanged(ref connectDisconnect, value);
        }

        private string FileName { get; set; }

        private ICommand ConnectCommand { get; }
        private ICommand SendMessageCommand { get; }
        private ICommand PickPictureCommand { get; }
    }
}
