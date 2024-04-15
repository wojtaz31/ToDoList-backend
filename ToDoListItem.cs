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