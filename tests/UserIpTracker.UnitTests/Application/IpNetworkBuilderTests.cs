using FluentAssertions;
using UserIpTracker.Application;
using Xunit;

namespace UserIpTracker.UnitTests.Application;

public class IpNetworkBuilderTests
{
    private readonly IpNetworkBuilder _builder;

    public IpNetworkBuilderTests()
    {
        _builder = new IpNetworkBuilder();    
    }

    [Theory]
    [InlineData("124", "124.0.0.0/8")]
    [InlineData("124.56", "124.56.0.0/16")]
    [InlineData("124.56.43", "124.56.43.0/24")]
    [InlineData("124.56.43.66", "124.56.43.66/32")]
    [InlineData("124.56.43.", "124.56.43.0/24")]
    public void Build_WhenIpv4PrefixProvided_ReturnsCorrectNetwork(string input, string expected)
    {
        var isSuccess = _builder.TryBuildIpv4(input, out var network);

        isSuccess.Should().BeTrue();
        network.Should().NotBeNull();   
        network.Should().BeEquivalentTo(expected);   
    }

    [Theory]
    [InlineData("2001", "2001::/16")]
    [InlineData("2001:db8", "2001:db8::/32")]
    [InlineData("2001:db8:abcd", "2001:db8:abcd::/48")]
    [InlineData("2001:db8:abcd:1", "2001:db8:abcd:1::/64")]
    public void Build_WhenIpv6PrefixProvided_ReturnsCorrectNetwork(
       string input,
       string expected)
    {
        var isSuccess = _builder.TryBuildIpv6(input, out var network);

        isSuccess.Should().BeTrue();
        network.Should().NotBeNull();
        network.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("124", "124.0.0.0/8")]
    [InlineData("2001:db8", "2001:db8::/32")]
    [InlineData("124.56", "124.56.0.0/16")]
    [InlineData("124.56.43", "124.56.43.0/24")]
    [InlineData("2001:db8:abcd", "2001:db8:abcd::/48")]
    [InlineData("124.56.43.66", "124.56.43.66/32")]
    [InlineData("2001:db8:abcd:1", "2001:db8:abcd:1::/64")]
    [InlineData("2001:db8:abcd:1:aa", "2001:db8:abcd:1:aa::/80")]
    public void Build_WhenUnknownIpPrefixProvided_ReturnsCorrectNetwork(string input, string expected)
    {
        var isSuccess = _builder.TryBuild(input, out var network);

        isSuccess.Should().BeTrue();
        network.Should().NotBeNull();
        network.Should().BeEquivalentTo(expected);
    }
}
