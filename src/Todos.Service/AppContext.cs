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
            todo.HasMany(x => x.Items)
                .WithOne(i => i.Owner)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
