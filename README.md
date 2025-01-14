# Task Manager Application

## Overview
This is a simple **Task Manager** application written in C#. It allows users to create, view, update, delete, search, and filter tasks. The application also calculates the task completion rate and persists tasks to a file for later use.

## Features
- **Add Task**: Add a new task with title, description, due date, priority, status, and category.
- **View Tasks**: View all tasks with optional filters (status, priority, category).
- **Update Task**: Update an existing task's details.
- **Delete Task**: Delete a task by title.
- **Search Tasks**: Search tasks by keyword (title or description).
- **Completion Rate**: View the percentage of completed tasks.
- **Persistence**: Save tasks to a file and load them automatically on restart.

## Setup and Usage

### Clone the Repository
```bash
git clone https://github.com/your-username/task-manager.git
cd task-manager
```

### Build and Run
1. Open the project folder in your terminal.
2. Run the following command to build and run the application:
   ```bash
   dotnet run
   ```

### Menu Options
When you run the application, you will see the following menu:
```
Task Manager Menu:
1. Add Task
2. View Tasks
3. Update Task
4. Delete Task
5. Search Tasks
6. View Completion Rate
7. Exit
Enter your choice:
```

### Add Task
Follow the prompts to add a new task. The application ensures that fields such as title, due date, and priority are valid before saving.

### View Tasks
You can view all tasks or filter them by:
- **Status**: "Pending", "In Progress", "Completed"
- **Priority**: "Low", "Medium", "High"
- **Category**: "Work", "Personal", "Study"

### Update Task
Provide the title of the task to update and follow the prompts to modify its fields. Leave a field blank to keep its current value.

### Delete Task
Enter the title of the task to delete it.

### Search Tasks
Enter a keyword to search for tasks by title or description.

### View Completion Rate
Displays the percentage of tasks that are marked as "Completed."

### Exit
Exits the application.

## File Storage
Tasks are saved in a file named `tasks.txt` in the application's root directory. The file is automatically updated whenever tasks are added, updated, or deleted.

## Example `tasks.txt` Format
```
Title,Description,DueDate,Priority,Status,Category
Task 1,Description 1,2025-01-14 14:00:00,High,Pending,Work
Task 2,Description 2,2025-01-15 16:30:00,Medium,Completed,Personal
```

