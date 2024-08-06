// See https://aka.ms/new-console-template for more information


using System.Net;
using System.Net.Sockets;
using System.Text;
using Server.Networking;


var ips = IP.AllIps.First();
var macResolver = new AddressToMac();
const int port = 9000;

var UdpClient = new UdpClient();
UdpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));

var endPoint = new IPEndPoint(IPAddress.Any, 0);

var task = Task.Run(() =>
{
    while (true)
    {
        byte[] data = UdpClient.Receive(ref endPoint);
        Console.WriteLine($"Received data from {endPoint.Address}:{endPoint.Port}");
        Console.WriteLine($"Data: {Encoding.UTF8.GetString(data)}");
        Console.WriteLine($"Mac: {macResolver.Resolve(endPoint.Address)}");
    }
});

task.Wait();


Console.ReadLine();