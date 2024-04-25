var builder = WebApplication.CreateBuilder();
builder.Services.AddSqlServer<AppContext>(builder.Configuration.GetConnectionString("Default"));
var app = builder.Build();
app.MapTodosApi();
app.MapDiagnosticsApi();
app.Run();
