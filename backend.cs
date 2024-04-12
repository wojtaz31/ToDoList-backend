using System.ComponentModel;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


public class ToDoListItem
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; }
    public string[] Tags { get; set; }

    public ToDoListItem(int id, string content, PriorityLevel priority = PriorityLevel.MEDIUM, DateTime? dueDate = null, string status = "Do zrobienia", string[] tags = null)
    {
        this.Id = id;
        this.Content = content;
        this.CreatedAt = DateTime.Now;
        this.Priority = priority;
        this.DueDate = dueDate;
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
