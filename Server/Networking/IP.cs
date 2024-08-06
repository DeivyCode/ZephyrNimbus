using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Server.Networking;

public class IP
{
    private readonly AddressToMac _addressToMac = new();

    public static IEnumerable<IPAddress> AllIps => GetIpsDefault();

    private static IEnumerable<IPAddress> GetIpsDefault()
    {
        foreach (var @networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            var props = networkInterface.GetIPProperties();

            //skip gateways
            var gateways = from ga in props.GatewayAddresses
                where
                    !ga.Address.Equals(IPAddress.Any)
                select true;

            if (!gateways.Any())
                continue;

            foreach (var uniAddr in props.UnicastAddresses)
            {
                var addr = uniAddr.Address;
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                    yield return addr;
            }
        }
    }
}