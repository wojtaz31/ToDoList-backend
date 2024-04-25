// Setup services
var builder = WebApplication.CreateBuilder();
builder.Services.AddSqlServer<AppContext>(builder.Configuration.GetConnectionString("Default"));
var app = builder.Build();
app.MapTodosApi();
app.Run();

public class AppContext : DbContext
{
    public DbSet<ToDoList> TodoLists { get;set;}

    public AppContext(DbContextOptions<AppContext> options): base(options)
    {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ToDoList>(todo => {
            todo.ToTable("TodoLists");
            todo.HasKey(x => x.Id);
            todo.Property(x => x.Name).HasMaxLength(30);
        });
    }
}

public class ToDoList
{
    public int Id { get; set; }
    public string Name { get;set; }
    public string Description { get;set;}
}


