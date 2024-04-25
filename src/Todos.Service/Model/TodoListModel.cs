using System.Text.Json.Serialization;

public class TodoListModel
{
    public TodoListModel(ToDoList list)
    {
        Name = list.Name;
        Description = list.Description;
    }

    [JsonConstructor]
    public TodoListModel(){}
    public string Name { get; set; }
    public string Description { get; set; }
}