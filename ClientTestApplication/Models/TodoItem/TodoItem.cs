using ReactiveUI;
using System;
using System.Windows.Input;

namespace ClientTestApplication.Models.TodoItem
{
    abstract class TodoItem
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public bool IsChecked { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string DueDateString
        {
            get => DueDate.ToString("dd:MM:yyyy");
        }
        public override string ToString()
        {
            return $"Id-{ID} :{IsChecked} {Description} {DueDate}";
        }
    }
}
