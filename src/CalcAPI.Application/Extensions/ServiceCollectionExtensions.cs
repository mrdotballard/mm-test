using CalcAPI.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CalcAPI.Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //dependency injection making avaiable the logger service to the controller
        services.AddScoped<ILogService, LogService>();
        return services;
    }
}