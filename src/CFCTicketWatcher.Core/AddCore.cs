using Microsoft.Extensions.DependencyInjection;

namespace CFCTicketWatcher.Core;

public static class AddCore
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Register core services here
        // e.g. services.AddScoped<IMyCoreService, MyCoreService>();

        return services;
    }

}
