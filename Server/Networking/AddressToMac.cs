using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Server.Networking;

internal partial class AddressToMac
{
    [GeneratedRegex(@"(?:[0-9A-F]{2}:){5}[0-9A-F]{2}")]
    private static partial Regex _regMac();

    private readonly ConcurrentDictionary<IPAddress, MacInfo> _macs = new();

    public static bool IsAcceptedMac(string mac) =>
        !string.IsNullOrWhiteSpace(mac) && _regMac().IsMatch(mac.Trim().ToUpperInvariant());


    public string Resolve(IPAddress ip)
    {
        //Check if the ip is a IPv4 family
        if (ip.AddressFamily != AddressFamily.InterNetwork)
            return string.Empty;


        if (_macs.TryGetValue(ip, out var macInfo))
            return macInfo.Mac;

        try
        {
            var mac = Arp.ParseProcNetArpAsync(ip).Result;
            if (mac == null) return string.Empty;
            
            _macs.TryAdd(ip, new MacInfo {Mac = mac.ToString(), Fresh = DateTime.Now});
            return mac.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return string.Empty;
    }
}

internal struct MacInfo
{
    public string Mac { get; set; }
    public DateTime Fresh { get; set; }
}