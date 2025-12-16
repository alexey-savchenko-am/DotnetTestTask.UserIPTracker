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

        services.AddDbContextFactory<UserDbContext>((provider, options) =>
        {
            var dbOptions = provider
                .GetRequiredService<IOptions<DatabaseOptions>>()
                .Value;

            options.UseNpgsql(dbOptions.ConnectionString, npgsql =>
            {
                if (dbOptions.MaxRetryCount is > 0)
                    npgsql.EnableRetryOnFailure();

                npgsql.CommandTimeout(dbOptions.CommandTimeout);
            });

            options.EnableDetailedErrors(dbOptions.EnableDetailedErrors);
            options.EnableSensitiveDataLogging(dbOptions.EnableSensitiveDataLogging);
        });

        services.AddScoped<IUserRepository, UserRepository>();

        SqlMapper.AddTypeHandler(new InetToIPAddressHandler());

        return services;
    }
}
