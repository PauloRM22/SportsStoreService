using Microsoft.EntityFrameworkCore;
using StoreService.Infrastructure.Data;

namespace SportsStoreService.Extensions;

public static class DatabaseExtensions
{
    public static void UseDatabaseMigrations(this IApplicationBuilder app)
    {
        var cfg = app.ApplicationServices.GetRequiredService<IConfiguration>();
        var logger = app.ApplicationServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("DatabaseMigrations");

        var inDocker = string.Equals(
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true",
            StringComparison.OrdinalIgnoreCase);

        var applyFlag = cfg.GetValue<bool?>("App:ApplyMigrations") ?? false;

        if (!inDocker && !applyFlag)
        {
            logger.LogInformation("Skip EF migrations (not in container and App:ApplyMigrations=false).");
            return;
        }

        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        const int maxRetries = 10;
        var delay = TimeSpan.FromSeconds(5);

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                db.Database.Migrate();
                logger.LogInformation("EF migrations applied successfully.");
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "EF migration attempt {Attempt}/{Max} failed. Retrying in {Delay}s...",
                    attempt, maxRetries, delay.TotalSeconds);
                Thread.Sleep(delay);
            }
        }

        db.Database.Migrate();
    }
}