using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

namespace Server.Networking;

public class Arp
{
    private const string ArpPathOnMAc = "/usr/sbin/arp";


    public static async Task<PhysicalAddress?> ParseProcNetArpAsync(IPAddress ip)
    {
        try
        {
            var start = new ProcessStartInfo()
            {
                FileName = ArpPathOnMAc,
                Arguments = "-an",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(start);

            if (process == null)
                return null;

            string output = await process.StandardOutput.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(output))
                return null;

            foreach (string line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!line.Contains(ip.ToString())) continue;
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 4)
                    continue;

                return PhysicalAddress.Parse(parts[3]);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return null;
    }
}