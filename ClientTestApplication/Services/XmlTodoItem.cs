using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using ClientTestApplication.Models.TodoItem;

namespace ClientTestApplication.Services
{
    static class XmlTodoItem
    {
        static string path = "";
        public static ObservableCollection<TodoItem> GetTodoItems(string userName)
        {
            path = $"TodoItems/{userName}.xml";
            ObservableCollection<TodoItem> todoItems = new ObservableCollection<TodoItem>();
            try
            {
                XDocument xmlDoc = XDocument.Load(path);

                foreach (XElement todoItemElement in xmlDoc.Element("TodoItems").Elements("TodoItem"))
                {
                    XElement description = todoItemElement.Element("Description");
                    XAttribute id = todoItemElement.Attribute("ID");
                    XElement isCheck = todoItemElement.Element("IsCheck");
                    XElement dueDate = todoItemElement.Element("DueDate");

                    if (description != null && id != null && isCheck != null && dueDate != null)
                    {
                        if (((DateTimeOffset)dueDate) < DateTime.Today)
                        {
                            todoItems.Add(new DueTodoItem { 
                                ID = id.Value,
                                Description = description.Value, 
                                DueDate = ((DateTimeOffset)dueDate), 
                                IsChecked = ((bool)isCheck)
                            });
                        }
                        else
                        {
                            todoItems.Add(new NotDueTodoItem
                            {
                                ID = id.Value,
                                Description = description.Value,
                                DueDate = ((DateTimeOffset)dueDate),
                                IsChecked = ((bool)isCheck)
                            });
                        }
                    }
                }

                return todoItems;
            }
            catch (DirectoryNotFoundException)
            {
                CreateDirectory();
                return todoItems;
            }
            catch (FileNotFoundException)
            {
                CreateFile();
                return todoItems;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return todoItems;
            }
        }

        public static void AddTodoItem(TodoItem todoItem)
        {
            XDocument xmlDoc = XDocument.Load(path);
            XElement root = xmlDoc.Element("TodoItems");

            root.Add(new XElement("TodoItem",
                new XAttribute("ID", todoItem.ID),
                new XElement("Description", todoItem.Description),
                new XElement("IsCheck", todoItem.IsChecked),
                new XElement("DueDate", todoItem.DueDate)
            ));

            xmlDoc.Save(path);
        }

        public static void SetCheck(TodoItem todoItem)
        {
            XDocument xmlDoc = XDocument.Load(path);

            foreach (XElement todoItemElement in xmlDoc.Element("TodoItems").Elements("TodoItem"))
            {
                if (todoItemElement.Attribute("ID").Value == todoItem.ID)
                {
                    todoItemElement.Element("IsCheck").Value = todoItem.IsChecked.ToString();
                }
            }

            xmlDoc.Save(path);
        }

        public static void DeleteItem(TodoItem todoItem)
        {
            XDocument xmlDoc = XDocument.Load(path);

            foreach (XElement todoItemElement in xmlDoc.Element("TodoItems").Elements("TodoItem"))
            {
                if (todoItemElement.Attribute("ID").Value == todoItem.ID)
                {
                    todoItemElement.Remove();
                }
            }

            xmlDoc.Save(path);
        }

        static void CreateDirectory()
        {
            XDocument xmlDoc;
            DirectoryInfo dirInfo = new DirectoryInfo("TodoItems");
            if (!dirInfo.Exists)
                dirInfo.Create();
            xmlDoc = new XDocument(new XElement("TodoItems"));
            xmlDoc.Save(path);
        }

        static void CreateFile()
        {
            XDocument xmlDoc;
            xmlDoc = new XDocument(new XElement("TodoItems"));
            xmlDoc.Save(path);
        }
    }
}
