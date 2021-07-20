using System;
using System.Collections.ObjectModel;
using ClientTestApplication.Models.TodoItem;
using System.Windows.Input;
using ReactiveUI;

namespace ClientTestApplication.ViewModels
{
    class TODOListViewModel : ViewModelBase
    {
        public TODOListViewModel()
        {
            TodoItems = new ObservableCollection<TodoItem>();

            var addEnable = this.WhenAnyValue(
                x => x.TodoItemTextBox,
                x => !string.IsNullOrWhiteSpace(x)
            );

            AddTodoItem = ReactiveCommand.Create(() =>
            {
                TodoItems.Add(new TodoItem { Description = TodoItemTextBox, DueDate = DueDate });
                TodoItemTextBox = string.Empty;
            }, addEnable);
        }

        private string todoItemTextBox = string.Empty;
        private DateTimeOffset dueDate = DateTimeOffset.Now;

        public ObservableCollection<TodoItem> TodoItems { get; set; }

        public string TodoItemTextBox
        {
            get => todoItemTextBox;
            private set => this.RaiseAndSetIfChanged(ref todoItemTextBox, value);
        }

        public DateTimeOffset DueDate
        {
            get => dueDate;
            private set => this.RaiseAndSetIfChanged(ref dueDate, value);
        }

        private ICommand AddTodoItem { get; }
    }
}
