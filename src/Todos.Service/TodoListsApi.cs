namespace MAVLink.Gateway.Service;

public static class TodoListsApi
{
    public static void MapTodosApi(this WebApplication app)
    {
        app.MapGet("/todos", async ([FromServices] AppContext context) =>
        {
            var lists = await context.TodoLists
                .Include(x => x.Items)
                .ToListAsync();
            return lists
                .Select(todoList => new TodoListModel(todoList));
        });

        app.MapPost("/todos", async ([FromServices] AppContext context, TodoListModel model) =>
        {
            var existingList = await context.TodoLists.FirstOrDefaultAsync(x => x.Name == model.Name);
            if (existingList != null) return Results.BadRequest($"Lista o nazwie '{model.Name}' już istnieje.");

            var newList = new ToDoList
            {
                Name = model.Name,
                Description = model.Description
            };
            context.TodoLists.Add(newList);
            await context.SaveChangesAsync();
            return Results.Created($"/todos", $"Utworzono nową listę o nazwie: {newList.Name}");
        });

        app.MapDelete("/todos/{name}", async ([FromServices] AppContext context, string name) =>
        {
            var list = await context.TodoLists.FirstOrDefaultAsync(t => t.Name == name);
            if (list == null) return Results.BadRequest($"Lista o nazwie {name} nie istnieje");
            context.TodoLists.Remove(list);
            await context.SaveChangesAsync();
            return Results.Ok($"Usunięto listę o nazwie: {name}");
        });

        app.MapDelete("/todos/{name}/removeAll", async ([FromServices] AppContext context, string name) =>
        {
            var list = await context.TodoLists.Include(l => l.Items).FirstOrDefaultAsync(t => t.Name == name);
            if (list == null) return Results.BadRequest($"Nie znaleziono listy o nazwie: {name}");
            list.Items.RemoveAll(e => true);
            await context.SaveChangesAsync();
    
            return Results.Ok($"Usunięto wszystkie zadania z listy o nazwie: {name}");
        });

        
        app.MapPost("/todos/{listName}/item", async ([FromServices] AppContext context, string listName, ToDoListItemModel model) =>
        {
            var list = await context.TodoLists.FirstOrDefaultAsync(t => t.Name == listName);
            if (list == null) return Results.BadRequest($"Nie znaleziono listy o nazwie: {listName}");
            var newItem = new ToDoListItem
            {
                Name = model.Name,
                Owner = list
            };
            list.Items.Add(newItem);
            await context.SaveChangesAsync();
            return Results.Created($"/todos/{listName}/item", $"Dodano nowe zadanie do listy o nazwie: {listName}");
        });
        
        app.MapDelete("/todos/{listName}/{item_name}", async ([FromServices] AppContext context, string listName, string itemName) =>
            {
                var list = await context.TodoLists.Include(l => l.Items).FirstOrDefaultAsync(t => t.Name == listName);
                if (list == null) return Results.BadRequest($"Nie znaleziono listy o nazwie: {listName}");
                
                var taskToRemove = list.Items.FirstOrDefault(task => task.Name == itemName);
                if (taskToRemove == null) return Results.BadRequest($"Nie znaleziono zadania o nazwie: {itemName} na liście: {listName}");

                list.Items.Remove(taskToRemove);
                await context.SaveChangesAsync();
                return Results.Ok($"Usunięto zadanie o nazwie: {itemName} z listy: {listName}");
            });

        app.MapGet("/todos/{listName}", async ([FromServices] AppContext context, string listName) =>
        {
            var list = await context.TodoLists.Include(l => l.Items).FirstOrDefaultAsync(t => t.Name == listName);
            if (list == null) return Results.BadRequest($"Nie znaleziono listy o nazwie: {listName}");
            
            return Results.Ok(list.Items.Select(task => new ToDoListItemModel(task.Name)));
        });
    }
}
