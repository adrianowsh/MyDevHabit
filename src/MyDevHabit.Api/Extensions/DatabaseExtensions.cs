using Microsoft.EntityFrameworkCore;
using MyDevHabit.Api.Database;

namespace MyDevHabit.Api.Extensions;

internal static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        await using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await using ApplicationIdentityDbContext identityContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        try
        {
            await context.Database.MigrateAsync();
            app.Logger.LogInformation("Application database migrations applied successfully.");

            await identityContext.Database.MigrateAsync();
            app.Logger.LogInformation("Identity database migrations applied successfully.");
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "An error occurred while applying database migrations");

            throw;
        }
    }
}
