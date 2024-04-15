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

