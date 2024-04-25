public class ToDoListItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ToDoList Owner { get; set; }
}