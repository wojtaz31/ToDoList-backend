using Microsoft.AspNetCore.Mvc;

[ApiController]
public class ToDoListController : ControllerBase
{
    [HttpPost("/list/{id}")]
    public IActionResult CreateList(int id)
    {
        if (ToDoListRepository.CheckIfListExists(id))
            return Conflict($"Lista o ID: {id} już istnieje !");

        ToDoList newList = new ToDoList(id);
        return Ok($"Utworzono listę o ID: {id}");
    }

    [HttpGet("/lists")]
    public IActionResult GetAllLists()
    {
        Dictionary<int, int> listLengths = ToDoListRepository.GetAllLists()
            .ToDictionary(entry => entry.Key, entry => entry.Value.TaskList.Count);

        var response = new Dictionary<string, Dictionary<int, int>> { { "Lists", listLengths } };
        return Ok(response);
    }

    [HttpDelete("/list/{id}")]
    public IActionResult DeleteList(int id)
    {
        if (!ToDoListRepository.CheckIfListExists(id))
            return NotFound($"Lista o ID: {id} nie istnieje !");

        ToDoListRepository.RemoveList(id);
        return Ok($"Usunięto listę o ID: {id}");
    }

    [HttpPost("/list/{id}/task")]
    public IActionResult CreateTask(int id, ToDoListItemDTO newItem)
    {
        if (!ToDoListRepository.CheckIfListExists(id))
            return NotFound($"Lista o ID: {id} nie istnieje !");

        PriorityLevel ParseResult;
        Enum.TryParse<PriorityLevel>(newItem.Priority, true, out ParseResult);

        ToDoList list = ToDoListRepository.GetAllLists()[id];
        if (!list.CreateTask(newItem, ParseResult))
            return Conflict($"Zadanie o ID: {newItem.Id} już istnieje w liście o ID: {id}");

        return Ok($"Dodano nowe zadanie o ID: {newItem.Id} do listy o ID: {id}");
    }

    [HttpDelete("/list/{listId}/task/{taskId}")]
    public IActionResult DeleteTask(int listId, int taskId)
    {
        if (!ToDoListRepository.CheckIfListExists(listId))
            return NotFound($"Lista o ID: {listId} nie istnieje !");

        ToDoList list = ToDoListRepository.GetAllLists()[listId];
        var itemToRemove = list.TaskList.FirstOrDefault(t => t.Id == taskId);
        if (itemToRemove == null)
            return NotFound($"Zadanie o ID: {taskId} nie istnieje w liście o ID: {listId} !");

        list.TaskList.Remove(itemToRemove);
        return Ok($"Usunięto zadanie o ID: {taskId} z listy o ID: {listId}");
    }

    [HttpPut("/list/{listId}/task/{taskId}")]
    public IActionResult UpdateTask(int listId, int taskId, ToDoListItemDTO updatedTask)
    {
        if (!ToDoListRepository.CheckIfListExists(listId))
            return NotFound($"Lista o ID: {listId} nie istnieje !");

        ToDoList list = ToDoListRepository.GetAllLists()[listId];
        if (!list.OverWriteTask(taskId, updatedTask))
            return Ok($"Dodano nowe zadanie o ID: {updatedTask.Id} do listy o ID: {listId}");

        return Ok($"Zaktualizowano zadanie o ID: {taskId} w liście o ID: {listId}");
    }

    [HttpGet("/list/{id}/tasks")]
    public IActionResult GetTasks(int id)
    {
        if (!ToDoListRepository.CheckIfListExists(id))
            return NotFound($"Lista o ID: {id} nie istnieje !");

        ToDoList list = ToDoListRepository.GetAllLists()[id];
        return Ok(list.TaskList);
    }
}