using System.ComponentModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/list/{id}", (int id) =>
{
    if (ToDoListRepository.CheckIfListExists(id)) return $"Lista o ID: {id} już istnieje !";
    ToDoList newList = new ToDoList(id);
    return $"Utworzono lisę o ID: {id}";
});

app.MapGet("/lists", () =>
{
    Dictionary<int, int> listLengths = ToDoListRepository.GetAllLists()
        .ToDictionary(entry => entry.Key, entry => entry.Value.TaskList.Count);

    var response = new Dictionary<string, Dictionary<int, int>> { { "Lists", listLengths } };
    return response;
});

app.MapDelete("/list/{id}", (int id) =>
{
    if (!ToDoListRepository.CheckIfListExists(id)) return $"Lista o ID: {id} nie istnieje !";
    ToDoListRepository.RemoveList(id);
    return $"Usunięto listę o ID: {id}";
});

app.MapPost("/list/{id}/task", (int id, ToDoListItemDTO newItem) =>
{
    if (!ToDoListRepository.CheckIfListExists(id)) return $"Lista o ID: {id} nie istnieje !";

    PriorityLevel ParseResult;
    Enum.TryParse<PriorityLevel>(newItem.Priority, true, out ParseResult);

    ToDoList list = ToDoListRepository.GetAllLists()[id];
    if (!list.CreateTask(newItem, ParseResult)) return $"Zadanie o ID: {newItem.Id} już istnieje w liście o ID: {id}";

    return $"Dodano nowe zadanie o ID: {newItem.Id} do listy o ID: {id}";
});

app.MapDelete("/list/{listId}/task/{taskId}", (int listId, int taskId) =>
{
    if (!ToDoListRepository.CheckIfListExists(listId)) return $"Lista o ID: {listId} nie istnieje !";
    ToDoList list = ToDoListRepository.GetAllLists()[listId];
    var itemToRemove = list.TaskList.FirstOrDefault(t => t.Id == taskId);
    if (itemToRemove == null) return $"Zadanie o ID: {taskId} nie istnieje w liście o ID: {listId} !";
    list.TaskList.Remove(itemToRemove);
    return $"Usunięto zadanie o ID: {taskId} z listy o ID: {listId}";
});

app.MapPut("/list/{listId}/task/{taskId}", (int listId, int taskId, ToDoListItemDTO updatedTask) =>
{
    if (!ToDoListRepository.CheckIfListExists(listId)) return $"Lista o ID: {listId} nie istnieje !";

    ToDoList list = ToDoListRepository.GetAllLists()[listId];
    if (!list.OverWriteTask(taskId, updatedTask)) return $"Dodano nowe zadanie o ID: {updatedTask.Id} do listy o ID: {listId}";

    return $"Zaktualizowano zadanie o ID: {taskId} w liście o ID: {listId}";
});


app.MapGet("/list/{id}/tasks", context =>
{
    int id = int.Parse(context.Request.RouteValues["id"].ToString());
    if (!ToDoListRepository.CheckIfListExists(id))
    {
        context.Response.StatusCode = 404;
        return context.Response.WriteAsync($"Lista o ID: {id} nie istnieje !");
    }

    ToDoList list = ToDoListRepository.GetAllLists()[id];

    return context.Response.WriteAsJsonAsync(list.TaskList);
});

app.Run();

public class ToDoListRepository
{
    private static Dictionary<int, ToDoList> lists = new Dictionary<int, ToDoList>();

    public static void AddList(ToDoList list) => lists.Add(list.id, list);
    public static void RemoveList(int id) => lists.Remove(id);
    public static Dictionary<int, ToDoList> GetAllLists() => lists;
    public static List<int> GetIds() => new List<int>(lists.Keys);
    public static bool CheckIfListExists(int id) => lists.ContainsKey(id);

}

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

public class ToDoListItemDTO
{
    public int Id { get; set; }
    public string Content { get; set; }
    public String? Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public string Status { get; set; }
    public string[] Tags { get; set; }
}

public class ToDoListItem
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public string Status { get; set; }
    public string[] Tags { get; set; }

    public ToDoListItem(int id, string content, PriorityLevel priority = PriorityLevel.MEDIUM, DateTime? deadline = null, string status = "Do zrobienia", string[] tags = null)
    {
        this.Id = id;
        this.Content = content;
        this.CreatedAt = DateTime.Now;
        this.Priority = priority;
        this.Deadline = deadline;
        this.Status = status;
        this.Tags = tags;
    }
}

public enum PriorityLevel
{
    [Description("HIGH")]
    HIGH,
    [Description("MEDIUM")]
    MEDIUM,
    [Description("LOW")]
    LOW
}