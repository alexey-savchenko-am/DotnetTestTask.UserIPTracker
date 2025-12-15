using Dapper;
using FormCollector.Infrastructure;
using FormCollector.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserIpTracker.Application.Abstract;
using UserIpTracker.Domain;
using UserIpTracker.Infrastructure.Data;
using UserIpTracker.Infrastructure.Data.Repositories;

namespace UserIpTracker.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.ConfigureOptions<UserDbOptionsSetup>();
        services.AddSingleton<IDbConnectionFactory, NpgConnectionFactory>();
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

        services.AddDbContext<DbContext, UserDbContext>((provider, builder) =>
        {
            var options = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            builder.UseNpgsql(options.ConnectionString, actions =>
            {
                if (options.MaxRetryCount is not null && options.MaxRetryCount > 0)
                {
                    actions.EnableRetryOnFailure();
                }
                actions.CommandTimeout(options.CommandTimeout);
            });
            builder.EnableDetailedErrors(options.EnableDetailedErrors);
            builder.EnableSensitiveDataLogging(options.EnableSensitiveDataLogging);
        });

        services.AddScoped<IUserRepository, UserRepository>();

        SqlMapper.AddTypeHandler(new InetToIPAddressHandler());

        return services;
    }
}
