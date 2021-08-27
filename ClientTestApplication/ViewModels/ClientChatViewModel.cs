using Avalonia.Layout;
using Avalonia.Controls;
using ClientTestApplication.Environments;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClientTestApplication.Models.ChatMessages;
using System;
using System.IO;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;

namespace ClientTestApplication.ViewModels
{
    class ClientChatViewModel : ViewModelBase
    {
        public ClientChatViewModel(string userName)
        {
            Messages = new ObservableCollection<Message>();


            ChatClient chatClient = new ChatClient(userName);
            chatClient.PrintMessage += ChatClient_PrintMessage;
            chatClient.PrintImage += ChatClient_PrintImage;

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
                    chatClient.SendMessage(InputMessage);
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

                    Bitmap bitmap = new Bitmap(FileName);
                    MemoryStream memoryStream = new MemoryStream();

                    bitmap.Save(memoryStream);

                    chatClient.SendImage(memoryStream.ToArray());
                }
            });
        }

        private async void ChatClient_PrintImage(byte[] imageData, HorizontalAlignment side)
        {
            if (side == HorizontalAlignment.Left)
            {
                await using (var stream = new MemoryStream(imageData))
                {
                    // NullRefException
                    Messages.Add(new IncommingImage { Source = await Task.Run(() => Bitmap.DecodeToWidth(stream, 400)) });
                }
            }
            else
            {
                await using (var stream = new MemoryStream(imageData))
                {
                    Messages.Add(new OutGoingImage { Source =  await Task.Run(() => Bitmap.DecodeToWidth(stream, 400)) });
                }
            }
        }

        private void ChatClient_PrintMessage(string message, HorizontalAlignment side)
        {
            if (side == HorizontalAlignment.Left)
            {
                Messages.Add(new IncommingMessage { MessageContent = message });
            }
            else
            {
                Messages.Add(new OutGoingMessage { MessageContent = message });
            }
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
