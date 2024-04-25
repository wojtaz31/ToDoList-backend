namespace MAVLink.Gateway.Service;

public static class TodoListsApi
{
    public static void MapTodosApi(this WebApplication app)
    {
        app.MapGet("/todos", async ([FromServices] AppContext context) =>
        {
            await context.TodoLists.ToListAsync();
        });
    }
}
