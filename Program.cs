using System.Globalization;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

class UserTask
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Priority { get; set; }
    public string Status { get; set; }
    public string Category { get; set; }

    public override string ToString()
    {
        return $"Title: {Title}\nDescription: {Description}\nCategory: {Category}\nPriority: {Priority}\nStatus: {Status}\nDue Date: {DueDate.ToString("g")}\n";
    }
}

class TaskManager
{
    public List<UserTask> tasks = new List<UserTask>();
    const string PATH = "tasks.txt";

    public async Task AddTask(UserTask task)
    {
        tasks.Add(task);
        string taskInfo = $"{task.Title},{task.Description},{task.DueDate:yyyy-MM-dd HH:mm:ss},{task.Priority},{task.Status},{task.Category}";
        await File.AppendAllTextAsync(PATH, taskInfo + Environment.NewLine);
        Console.WriteLine("Task added successfully!");
    }

    public async Task LoadTasksFromFile()
    {
        tasks.Clear();

        if (!File.Exists(PATH))
        {
            Console.WriteLine("Task file does not exist. Starting with an empty task list.");
            return;
        }

        string[] lines = await File.ReadAllLinesAsync(PATH);
        foreach (string line in lines)
        {
            string[] taskInfo = line.Split(',');
            if (taskInfo.Length < 6) throw new FormatException("Each line must have at least 6 fields.");

            UserTask t = new UserTask
            {
                Title = taskInfo[0],
                Description = taskInfo[1],
                Priority = taskInfo[3],
                Status = taskInfo[4],
                Category = taskInfo[5]
            };

            if (!DateTime.TryParseExact(taskInfo[2], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                throw new FormatException($"The string {taskInfo[2]} is not a valid date.");

            t.DueDate = dateValue;
            tasks.Add(t);
        }
    }

    public async Task DisplayTasks(string? filterType = null, string? filter = null)
    {
        await LoadTasksFromFile();
        List<UserTask> result = tasks;

        if (string.IsNullOrEmpty(filter) || string.IsNullOrEmpty(filterType)) Console.WriteLine("No filter applied");

        if (filterType == "status")
            result = tasks.Where(task => task.Status == filter).ToList();
        else if (filterType == "priority")
            result = tasks.Where(task => task.Priority == filter).ToList();
        else if (filterType == "category")
            result = tasks.Where(task => task.Category == filter).ToList();
        else if (!(string.IsNullOrEmpty(filter) || string.IsNullOrEmpty(filterType)))
            Console.WriteLine("Invalid filter type. Displaying all tasks.");

        if (!result.Any())
        {
            Console.WriteLine("No tasks found.");
            return;
        }

        foreach (UserTask t in result)
        {
            Console.WriteLine(t.ToString());
            Console.WriteLine("*********************************************************************************");
        }
    }

    public async Task SaveTasksToFile()
    {
        await File.WriteAllTextAsync(PATH, string.Empty);
        foreach (UserTask task in tasks)
        {
            string taskInfo = $"{task.Title},{task.Description},{task.DueDate.ToString("yyyy-MM-dd HH:mm:ss")},{task.Priority},{task.Status},{task.Category}";
            await File.AppendAllTextAsync(PATH, taskInfo + Environment.NewLine);
        }
    }

    public async Task DeleteTask(string Title)
    {
        await LoadTasksFromFile();
        UserTask? taskToDelete = tasks.FirstOrDefault(task => task.Title.Equals(Title, StringComparison.OrdinalIgnoreCase));
        if (taskToDelete == null)
        {
            Console.WriteLine("Task not found.");
            return;
        }
        tasks.Remove(taskToDelete);
        await SaveTasksToFile();
        Console.WriteLine("Task deleted successfully!");
    }

    public async Task UpdateTask(string Title)
    {
        tasks.Clear();
        await LoadTasksFromFile();
        UserTask? taskToUpdate = tasks.FirstOrDefault(task => task.Title.Equals(Title, StringComparison.OrdinalIgnoreCase));
        if (taskToUpdate == null)
        {
            Console.WriteLine("Task not found.");
            return;
        }
        tasks.Remove(taskToUpdate);
        Console.WriteLine("Leave fields empty to keep current value.");

        Console.Write("New Title: ");
        string newTitle = Console.ReadLine();
        if (!string.IsNullOrEmpty(newTitle)) taskToUpdate.Title = newTitle;

        Console.Write("New Description: ");
        string newDescription = Console.ReadLine();
        if (!string.IsNullOrEmpty(newDescription)) taskToUpdate.Description = newDescription;

        Console.Write("New Due Date (yyyy-MM-dd HH:mm): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime newDueDate)) taskToUpdate.DueDate = newDueDate;

        Console.Write("New Priority (Low, Medium, High): ");
        string newPriority = Console.ReadLine();
        if (!string.IsNullOrEmpty(newPriority)) taskToUpdate.Priority = newPriority;

        Console.Write("New Status (Pending, In Progress, Completed): ");
        string newStatus = Console.ReadLine();
        if (!string.IsNullOrEmpty(newStatus)) taskToUpdate.Status = newStatus;

        tasks.Add(taskToUpdate);
        await SaveTasksToFile();
        Console.WriteLine("Task updated successfully!");
    }

    public async Task DisplayCompletionRate()
    {
        await LoadTasksFromFile();
        int totalTasks = tasks.Count;
        int completedTasks = tasks.Count(t => t.Status == "Completed");
        if (totalTasks == 0)
        {
            Console.WriteLine("No tasks available to calculate completion rate.");
            return;
        }
        double completionRate = (completedTasks / (double)totalTasks) * 100;
        Console.WriteLine($"Completion Rate: {completionRate:F2}%");
    }

    public async Task SearchTasks(string keyword)
    {
        await LoadTasksFromFile();

        var searchResults = tasks.Where(t =>
            t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            t.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase));

        if (!searchResults.Any())
        {
            Console.WriteLine("No tasks found matching the keyword.");
            return;
        }

        foreach (UserTask t in searchResults)
        {
            Console.WriteLine(t.ToString());
            Console.WriteLine("*********************************************************************************");
        }
    }
}

public class Program
{
    public static async Task Main()
    {
        TaskManager taskManager = new TaskManager();
        string choice;

        do
        {
            Console.WriteLine("\nTask Manager Menu:");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. View Tasks");
            Console.WriteLine("3. Update Task");
            Console.WriteLine("4. Delete Task");
            Console.WriteLine("5. Search Tasks");
            Console.WriteLine("6. View Completion Rate");
            Console.WriteLine("7. Exit");
            Console.Write("Enter your choice: ");
            choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AddNewTask(taskManager);
                    break;
                case "2":
                    await ViewTasks(taskManager);
                    break;
                case "3":
                    await UpdateTask(taskManager);
                    break;
                case "4":
                    await DeleteTask(taskManager);
                    break;
                case "5":
                    await SearchTasks(taskManager);
                    break;
                case "6":
                    await taskManager.DisplayCompletionRate();
                    break;
                case "7":
                    Console.WriteLine("Exiting...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        } while (choice != "7");
    }

    static async Task AddNewTask(TaskManager taskManager)
    {
        string title;
        string description;
        DateTime dueDate;
        string priority;
        string status;
        string category;

        do
        {
            Console.Write("Enter Title: ");
            title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Title cannot be empty. Please try again.");
            }
            else if (taskManager.tasks.Any(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("A task with this title already exists. Please choose a different title.");
            }
            else
            {
                break;
            }
        } while (string.IsNullOrWhiteSpace(title) || taskManager.tasks.Any(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase)));


        do
        {
            Console.Write("Enter Description: ");
            description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
                Console.WriteLine("Description cannot be empty. Please try again.");
        } while (string.IsNullOrWhiteSpace(description));

        do
        {
            Console.Write("Enter Due Date (yyyy-MM-dd HH:mm): ");
            if (!DateTime.TryParse(Console.ReadLine(), out dueDate))
                Console.WriteLine("Invalid date format. Please try again (yyyy-MM-dd HH:mm).");
        } while (dueDate == default);

        do
        {
            Console.Write("Enter Priority (Low, Medium, High): ");
            priority = Console.ReadLine();
            if (priority != "Low" && priority != "Medium" && priority != "High")
                Console.WriteLine("Priority must be 'Low', 'Medium', or 'High'. Please try again.");
        } while (priority != "Low" && priority != "Medium" && priority != "High");

        do
        {
            Console.Write("Enter Status (Pending, In Progress, Completed): ");
            status = Console.ReadLine();
            if (status != "Pending" && status != "In Progress" && status != "Completed")
                Console.WriteLine("Status must be 'Pending', 'In Progress', or 'Completed'. Please try again.");
        } while (status != "Pending" && status != "In Progress" && status != "Completed");

        do
        {
            Console.Write("Enter Category (Work, Personal, Study): ");
            category = Console.ReadLine();
            if (category != "Work" && category != "Personal" && category != "Study")
                Console.WriteLine("Category must be 'Work', 'Personal', or 'Study'. Please try again.");
        } while (category != "Work" && category != "Personal" && category != "Study");

        UserTask task = new UserTask
        {
            Title = title,
            Description = description,
            DueDate = dueDate,
            Priority = priority,
            Status = status,
            Category = category
        };

        await taskManager.AddTask(task);
    }


    static async Task ViewTasks(TaskManager taskManager)
    {
        Console.Write("Filter by (status/priority/category/none): ");
        string filterType = Console.ReadLine()?.ToLower();

        string filter = null;
        if (filterType == "status" || filterType == "priority" || filterType == "category")
        {
            Console.Write($"Enter {filterType}: ");
            filter = Console.ReadLine();
        }

        await taskManager.DisplayTasks(filterType, filter);
    }

    static async Task UpdateTask(TaskManager taskManager)
    {
        Console.Write("Enter Task Title to update: ");
        string title = Console.ReadLine();
        await taskManager.UpdateTask(title);
    }

    static async Task DeleteTask(TaskManager taskManager)
    {
        Console.Write("Enter Task Title to delete: ");
        string title = Console.ReadLine();
        await taskManager.DeleteTask(title);
    }

    static async Task SearchTasks(TaskManager taskManager)
    {
        Console.Write("Enter keyword to search: ");
        string keyword = Console.ReadLine();
        await taskManager.SearchTasks(keyword);
    }
}
