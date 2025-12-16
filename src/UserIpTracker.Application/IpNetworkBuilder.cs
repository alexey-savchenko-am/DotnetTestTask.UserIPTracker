using UserIpTracker.Application.Abstract;

namespace UserIpTracker.Application;

internal sealed class IpNetworkBuilder
    : IIpNetworkBuilder
{
    public bool TryBuild(string input, out string network)
    {
        network = null;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (TryBuildIpv4(input, out network))
            return true;

        if (TryBuildIpv6(input, out network))
            return true;

        return false;
    }

    public bool TryBuildIpv4(string input, out string network)
    {
        network = null;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var parts = input.Split('.', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is < 1 or > 4)
            return false;

        var octets = new int[4];

        for (int i = 0; i < parts.Length; i++)
        {
            if (!int.TryParse(parts[i], out var value))
                return false;

            if (value < 0 || value > 255)
                return false;

            octets[i] = value;
        }

        var mask = parts.Length * 8;

        network = $"{octets[0]}.{octets[1]}.{octets[2]}.{octets[3]}/{mask}";
        return true;
    }

    public bool TryBuildIpv6(string input, out string network)
    {
        network = null;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var parts = input.Split(':', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is < 1 or > 8)
            return false;

        foreach (var part in parts)
        {
            if (part.Length == 0 || part.Length > 4)
                return false;

            if (!ushort.TryParse(part, System.Globalization.NumberStyles.HexNumber, null, out _))
                return false;
        }

        var mask = parts.Length * 16;
        network = $"{string.Join(':', parts)}::/{mask}";
        return true;
    }
}
