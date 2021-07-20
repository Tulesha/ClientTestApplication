using System;

namespace ClientTestApplication.Models.TodoItem
{
    class TodoItem
    {
        public string Description { get; set; }
        public bool IsChecked { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string DueDateString
        {
            get => DueDate.ToString("dd:MM:yyyy");
        }
    }
}
