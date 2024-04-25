namespace MAVLink.Gateway.Service;

public static class DiagnosticsApi
{
    public static void MapDiagnosticsApi(this WebApplication app)
    {
        app.MapGet("/status", (IConfiguration configuration) => 
            Results.Ok(new
        {
            stamp = DateTime.UtcNow,
            banner = configuration["WelcomeBanner"]
        }));
    }
}
