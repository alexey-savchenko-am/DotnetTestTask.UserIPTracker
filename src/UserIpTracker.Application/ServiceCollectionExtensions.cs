using Microsoft.Extensions.DependencyInjection;
using UserIpTracker.Application.Abstract;

namespace UserIpTracker.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        });

        services.AddSingleton<IIpNetworkBuilder, IpNetworkBuilder>();

        return services;
    }
}
