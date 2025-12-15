using System.Net;
using System.Net.Sockets;
using UserIpTracker.Application.Abstract;

namespace UserIpTracker.Application;

internal sealed class IpNetworkBuilder
    : IIpNetworkBuilder
{
    public bool TryBuild(string input, out string network)
    {
        network = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        // Full IP (IPv4 or IPv6)
        if (IPAddress.TryParse(input, out var ip))
        {
            var prefix = ip.AddressFamily == AddressFamily.InterNetwork
                ? 32
                : 128;

            network = $"{ip}/{prefix}";
            return true;
        }

        // IPv4 prefix
        if (input.Contains('.', StringComparison.Ordinal))
        {
            var parts = input.Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length is < 1 or > 4)
                return false;

            if (!parts.All(p => byte.TryParse(p, out _)))
                return false;

            var prefix = parts.Length * 8;

            var padded = parts
                .Concat(Enumerable.Repeat("0", 4 - parts.Length));

            network = $"{string.Join('.', padded)}/{prefix}";
            return true;
        }

        // IPv6 prefix
        if (input.Contains(':', StringComparison.Ordinal))
        {
            var expanded = input.EndsWith("::", StringComparison.Ordinal)
                ? input
                : $"{input}::";

            if (!IPAddress.TryParse(expanded, out var ipv6))
                return false;

            var groups = input
                .Split(':', StringSplitOptions.RemoveEmptyEntries)
                .Length;

            var prefix = Math.Min(groups * 16, 128);

            network = $"{ipv6}/{prefix}";
            return true;
        }

        return false;
    }
}
