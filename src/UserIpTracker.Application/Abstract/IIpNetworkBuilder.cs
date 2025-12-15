namespace UserIpTracker.Application.Abstract;

public interface IIpNetworkBuilder
{
    bool TryBuild(string input, out string network);
}
