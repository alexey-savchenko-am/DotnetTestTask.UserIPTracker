using Dapper;
using NpgsqlTypes;
using System.Data;
using System.Net;

namespace UserIpTracker.Infrastructure;

public sealed class InetToIPAddressHandler
    : SqlMapper.TypeHandler<IPAddress>
{
    public override void SetValue(IDbDataParameter parameter, IPAddress value)
    {
        parameter.Value = value.ToString();
    }

    public override IPAddress Parse(object value)
    {
        return value switch
        {
            string s => IPAddress.Parse(s),
            NpgsqlInet inet => inet.Address,
            _ => IPAddress.Parse(value.ToString()!)
        };
    }
}
