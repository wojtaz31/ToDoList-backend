using System.ComponentModel;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/list/{id}", (int id) =>
{
    if (ToDoList.CheckIfListExists(id)) return $"Lista o ID: {id} już istnieje !";
    ToDoList newList = new ToDoList(id);
    return $"Utworzono lisę o ID: {id}";
});

app.Run();

public class ToDoList
{
    private static List<int> idList = new List<int>();
    public int id { get; set; }
    public List<ToDoListItem> TaskList { get; set; }

    public ToDoList(int id)
    {
        this.id = id;
        this.TaskList = new List<ToDoListItem>();
        idList.Add(id);
    }

    public static List<int> GetIds() => idList;

    public static bool CheckIfListExists(int id) => ToDoList.GetIds().Contains(id);
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