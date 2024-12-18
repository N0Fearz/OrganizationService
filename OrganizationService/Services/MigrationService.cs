using Microsoft.EntityFrameworkCore;
using OrganizationService.Data;

namespace OrganizationService.Services;

public class MigrationService
{
    private readonly IConfiguration _configuration;
    public MigrationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task MigrateAsync()
    {
        var connectionString = _configuration.GetConnectionString("OrganizationsDB");
        var optionsBuilder = new DbContextOptionsBuilder<OrganizationContext>();
        optionsBuilder.UseNpgsql(connectionString);
        var dbContext = new OrganizationContext(optionsBuilder.Options);

        // Check if the migrations are needed
        if (await dbContext.Database.GetPendingMigrationsAsync() is { } migrations && migrations.Any())
        {
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Migrated database");
        }
    }
}