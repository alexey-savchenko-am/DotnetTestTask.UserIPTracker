using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using UserIpTracker.Infrastructure;

namespace FormCollector.Infrastructure;

internal class UserDbOptionsSetup
    : IConfigureOptions<DatabaseOptions>
{
    private const string DbName = "UserDb";
    private const string ConfigurationSectionName = "DatabaseOptions";
    private readonly IConfiguration _configuration;

    public UserDbOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(DatabaseOptions options)
    {
        var connectionString = _configuration.GetConnectionString(DbName);

        if (connectionString is not null)
        {
            options.ConnectionString = connectionString;
        }

        _configuration.GetSection(ConfigurationSectionName).Bind(options);
    }
}
