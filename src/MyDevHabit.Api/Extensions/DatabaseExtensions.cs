﻿using Microsoft.EntityFrameworkCore;
using MyDevHabit.Api.Database;

namespace MyDevHabit.Api.Extensions;

internal static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await context.Database.MigrateAsync();

            app.Logger.LogInformation("Database migrations applied successfully.");
        }
        catch
        {

            app.Logger.LogInformation("Database migrations applied successfully.");
            throw;
        }
    }
}
