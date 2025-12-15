using System.Data;

namespace UserIpTracker.Application.Abstract;

public interface IDbConnectionFactory
{
    IDbConnection GetConnection();
}
