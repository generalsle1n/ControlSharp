using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ControlSharp.Client.Helper;

public class DeviceIDGenerator
{
    public static async Task<string> GenerateAsync()
    {
        string hostname = Dns.GetHostName();
        using (SHA256 Hasher = SHA256.Create())
        {
            byte[] hostnameData = Encoding.UTF8.GetBytes(hostname);
            byte[] hash = Hasher.ComputeHash(hostnameData);
            return Encoding.UTF8.GetString(hash);
        }
    }
}