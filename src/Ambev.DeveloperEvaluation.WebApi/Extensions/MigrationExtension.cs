using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Extensions;

public static class MigrationExtension
{
    public static void ApplyMigrations(
        this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        var connection =
            context.Database.GetConnectionString();

        if (context.Database.GetPendingMigrations().Any())
            context.Database.Migrate();
    }
}
