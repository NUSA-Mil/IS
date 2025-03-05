using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Info_System
{
    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    class TaskItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
    }

    class UserManager
    {
        private const string UserFile = "users.txt";

        public void RegisterUser(string username, string password)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(UserFile, true))
                {
                    writer.WriteLine($"{username}:{password}");
                }
                Console.WriteLine("Регистрация успешна!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при регистрации пользователя: {ex.Message}");
            }
        }

        public bool AuthenticateUser(string username, string password)
        {
            if (!File.Exists(UserFile)) return false;

            try
            {
                var lines = File.ReadAllLines(UserFile);
                return lines.Any(line =>
                {
                    var parts = line.Split(':');
                    return parts[0] == username && parts[1] == password;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при аутентификации: {ex.Message}");
                return false;
            }
        }
    }

    class TaskManager
    {
        private const string TaskFile = "tasks.txt";

        public void AddTask(TaskItem task)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(TaskFile, true))
                {
                    writer.WriteLine($"{task.Title}:{task.Description}:{task.Priority}:{task.Status}");
                }
                Console.WriteLine("Задача добавлена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении задачи: {ex.Message}");
            }
        }

        public List<TaskItem> GetTasks()
        {
            var tasks = new List<TaskItem>();
            if (!File.Exists(TaskFile)) return tasks;

            try
            {
                var lines = File.ReadAllLines(TaskFile);
                foreach (var line in lines)
                {
                    var parts = line.Split(':');
                    tasks.Add(new TaskItem
                    {
                        Title = parts[0],
                        Description = parts[1],
                        Priority = parts[2],
                        Status = parts[3]
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении задач: {ex.Message}");
            }
            return tasks;
        }

        public TaskItem FindTask(string title)
        {
            var tasks = GetTasks();
            return tasks.FirstOrDefault(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        public void EditTask(string title, TaskItem updatedTask)
        {
            var existingTask = FindTask(title);
            if (existingTask == null)
            {
                Console.WriteLine("Задача с таким заголовком не найдена.");
                return;
            }

            var tasks = GetTasks();
            try
            {
                using (StreamWriter writer = new StreamWriter(TaskFile, false))
                {
                    foreach (var task in tasks)
                    {
                        if (task.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
                        {
                            task.Title = updatedTask.Title;
                            task.Description = updatedTask.Description;
                            task.Priority = updatedTask.Priority;
                            task.Status = updatedTask.Status;
                        }
                        writer.WriteLine($"{task.Title}:{task.Description}:{task.Priority}:{task.Status}");
                    }
                }
                Console.WriteLine("Задача обновлена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при редактировании задачи: {ex.Message}");
            }
        }

        public void DeleteTask(string title)
        {
            var existingTask = FindTask(title);
            if (existingTask == null)
            {
                Console.WriteLine("Задача с таким заголовком не найдена.");
                return;
            }

            var tasks = GetTasks();
            try
            {
                using (StreamWriter writer = new StreamWriter(TaskFile, false))
                {
                    foreach (var task in tasks)
                    {
                        if (!task.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
                        {
                            writer.WriteLine($"{task.Title}:{task.Description}:{task.Priority}:{task.Status}");
                        }
                    }
                }
                Console.WriteLine("Задача удалена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении задачи: {ex.Message}");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            UserManager userManager = new UserManager();
            TaskManager taskManager = new TaskManager();

            Console.WriteLine("1. Регистрация\n2. Вход");
            var choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Введите имя пользователя: ");
                string username = Console.ReadLine();
                Console.Write("Введите пароль: ");
                string password = Console.ReadLine();
                userManager.RegisterUser(username, password);
            }
            else if (choice == "2")
            {
                Console.Write("Введите имя пользователя: ");
                string username = Console.ReadLine();
                Console.Write("Введите пароль: ");
                string password = Console.ReadLine();
                if (userManager.AuthenticateUser(username, password))
                {
                    Console.WriteLine("Аутентификация успешна!");
                    ManageTasks(taskManager);
                }
                else
                {
                    Console.WriteLine("Неверное имя пользователя или пароль.");
                }
            }
        }

        static void ManageTasks(TaskManager taskManager)
        {
            while (true)
            {
                Console.WriteLine("1. Добавить задачу\n2. Просмотреть задачи\n3. Редактировать задачу\n4. Удалить задачу\n5. Выход");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        var newTask = CreateTaskFromInput();
                        taskManager.AddTask(newTask);
                        break;
                    case "2":
                        DisplayTasks(taskManager.GetTasks());
                        break;
                    case "3":
                        Console.Write("Введите заголовок задачи для редактирования: ");
                        string titleToEdit = Console.ReadLine();
                        var updatedTask = CreateTaskFromInput();
                        taskManager.EditTask(titleToEdit, updatedTask);
                        break;
                    case "4":
                        Console.Write("Введите заголовок задачи для удаления: ");
                        string titleToDelete = Console.ReadLine();
                        taskManager.DeleteTask(titleToDelete);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Некорректный выбор. Пожалуйста, попробуйте снова.");
                        break;
                }
            }
        }

        static TaskItem CreateTaskFromInput()
        {
            var task = new TaskItem();
            Console.Write("Введите заголовок задачи: ");
            task.Title = Console.ReadLine();
            Console.Write("Введите описание задачи: ");
            task.Description = Console.ReadLine();
            Console.Write("Введите приоритет (Низкий, Средний, Высокий): ");
            task.Priority = Console.ReadLine();
            Console.Write("Введите статус (Недоступна, В процессе, Завершена): ");
            task.Status = Console.ReadLine();
            return task;
        }

        static void DisplayTasks(List<TaskItem> tasks)
        {
            Console.WriteLine("Список задач:");
            foreach (var task in tasks)
            {
                Console.WriteLine($"Заголовок: {task.Title}, Описание: {task.Description}, Приоритет: {task.Priority}, Статус: {task.Status}");
            }
        }
    }
}
