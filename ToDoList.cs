public class ToDoList
{
    public int id { get; set; }
    public List<ToDoListItem> TaskList { get; set; }

    public ToDoList(int id)
    {
        this.id = id;
        this.TaskList = new List<ToDoListItem>();
        ToDoListRepository.AddList(this);
    }

    public bool CreateTask(ToDoListItemDTO newItem, PriorityLevel ParseResult)
    {
        if (this.TaskList.FirstOrDefault(t => t.Id == newItem.Id) != null) return false;
        var newTask = new ToDoListItem(
        id: newItem.Id,
        content: newItem.Content,
        priority: ParseResult,
        deadline: newItem.Deadline,
        status: newItem.Status,
        tags: newItem.Tags
    );

        this.TaskList.Add(newTask);
        return true;
    }

    public bool OverWriteTask(int taskId, ToDoListItemDTO updatedTask)
    {
        ToDoListItem existingTask = this.TaskList.FirstOrDefault(t => t.Id == taskId);

        PriorityLevel parseResult;
        Enum.TryParse<PriorityLevel>(updatedTask.Priority, true, out parseResult);

        if (existingTask == null)
        {
            this.CreateTask(updatedTask, parseResult);
            return false;
        }

        existingTask.Content = updatedTask.Content;
        existingTask.Priority = parseResult;
        existingTask.Deadline = updatedTask.Deadline;
        existingTask.Status = updatedTask.Status;
        existingTask.Tags = updatedTask.Tags;
        return true;
    }
}