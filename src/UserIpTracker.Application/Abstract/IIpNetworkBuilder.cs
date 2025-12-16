namespace UserIpTracker.Application.Abstract;

public interface IIpNetworkBuilder
{
    bool TryBuild(string input, out string network);

    bool TryBuildIpv4(string input, out string network);

    bool TryBuildIpv6(string input, out string network);
}
