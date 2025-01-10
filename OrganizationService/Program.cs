using Keycloak.AuthServices.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrganizationService.Data;
using OrganizationService.Repository;
using OrganizationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var configuration = builder.Configuration;

builder.Services.AddTransient<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IOrganizationService, OrganizationService.Services.OrganizationService>();
builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddHostedService<OrganizationCreationRabbitMqConsumer>();
builder.Services.AddHostedService<OrganizationCheckRabbitMqconsumer>();
builder.Services.AddHostedService<OrganizationRemovalRabbitMqConsumer>();
builder.Services.AddSingleton<OrganizationCreationRabbitMqSender>();
builder.Services.AddSingleton<OrganizationRemovalRabbitMqSender>();
builder.Services.AddScoped<MigrationService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKeycloakWebApiAuthentication(configuration);

builder.Services.AddDbContextPool<OrganizationContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("OrganizationsDB"),
        o => o
            .SetPostgresVersion(17, 0)));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
