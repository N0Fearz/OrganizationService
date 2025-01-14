using Microsoft.EntityFrameworkCore;
using OrganizationService.Data;
using OrganizationService.Models;

namespace OrganizationService.Services;

public class MigrationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogPublisher _logPublisher;
    public MigrationService(IConfiguration configuration, ILogPublisher logPublisher)
    {
        _configuration = configuration;
        _logPublisher = logPublisher;
        
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
            
            _logPublisher.SendMessage(new LogMessage
            {
                ServiceName = "OrganizationService",
                LogLevel = "Information",
                Message = "Schema migrated successfully.",
                Timestamp = DateTime.Now
            });
            Console.WriteLine("Migrated database");
        }
    }
}