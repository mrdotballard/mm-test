using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using CalcAPI.Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using CalcAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CalcAPI.Domain.Interfaces;
using CalcAPI.Infrastructure.Repositories;

namespace CalcAPI.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add authentication scheme to the service collection for the use of the ApiKeyAuthenticationHandler
        services.AddAuthentication("ApiKey")
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

        // Add the DbContext for use by the LoggingDbContext
        services.AddDbContext<LoggingDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("SQLiteDefault")));

        // Add the repository for use by the logging service
        services.AddScoped<ILogRepository, LogRepository>();
        return services;
    }
}