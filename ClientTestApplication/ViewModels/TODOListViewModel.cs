using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;

using ClientTestApplication.Models.TodoItem;
using ClientTestApplication.Services;
using System.Linq;

namespace ClientTestApplication.ViewModels
{
    class TODOListViewModel : ViewModelBase
    {
        public TODOListViewModel(string userName)
        {
            TodoItems = XmlTodoItem.GetTodoItems(userName);

            var addEnable = this.WhenAnyValue(
                x => x.TodoItemTextBox,
                x => !string.IsNullOrWhiteSpace(x)
            );

            AddTodoItem = ReactiveCommand.Create(() =>
            {
                if (DueDate < DateTime.Today)
                {
                    var todoItem = new DueTodoItem
                    {
                        ID = System.Guid.NewGuid().ToString(), 
                        Description = TodoItemTextBox, 
                        DueDate = DueDate 
                    };

                    TodoItems.Add(todoItem);
                    XmlTodoItem.AddTodoItem(todoItem);
                }
                else
                {
                    var todoItem = new NotDueTodoItem { 
                        ID = System.Guid.NewGuid().ToString(), 
                        Description = TodoItemTextBox, 
                        DueDate = DueDate 
                    };

                    TodoItems.Add(todoItem);
                    XmlTodoItem.AddTodoItem(todoItem);
                }
                TodoItemTextBox = string.Empty;
            }, addEnable);

            ClickChecked = ReactiveCommand.Create((TodoItem item) =>
            {
                XmlTodoItem.SetCheck(item);
            });


            ClickDeleteItem = ReactiveCommand.Create((TodoItem item) =>
            {
                TodoItems.Remove(item);
                XmlTodoItem.DeleteItem(item);
            });

            SortItems = ReactiveCommand.Create((string direction) =>
            {
                if (direction == "up")
                {
                    var sorted = new ObservableCollection<TodoItem>(TodoItems.OrderBy(i => i.DueDate));
                    TodoItems.Clear();

                    foreach (var item in sorted)
                    {
                        TodoItems.Add(item);
                    }
                }
                else
                {
                    var sorted = new ObservableCollection<TodoItem>(TodoItems.OrderBy(i => i.DueDate).Reverse());
                    TodoItems.Clear();

                    foreach (var item in sorted)
                    {
                        TodoItems.Add(item);
                    }
                }
            });
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
        private ICommand ClickChecked { get; }
        private ICommand ClickDeleteItem { get; }
        private ICommand SortItems { get; }
    }
}
