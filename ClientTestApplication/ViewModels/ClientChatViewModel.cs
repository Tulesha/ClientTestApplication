using Avalonia.Layout;
using ClientTestApplication.Environments;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ClientTestApplication.Models.ChatMessages;

namespace ClientTestApplication.ViewModels
{
    class ClientChatViewModel : ViewModelBase
    {
        public ClientChatViewModel(string userName)
        {
            Messages = new ObservableCollection<Message>();


            ChatClient chatClient = new ChatClient(userName);
            chatClient.PrintMessage += ChatClient_PrintMessage;

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

        private ICommand ConnectCommand { get; }
        private ICommand SendMessageCommand { get; }
    }
}
