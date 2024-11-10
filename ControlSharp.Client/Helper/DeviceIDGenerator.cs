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
            
            StringBuilder Data = new StringBuilder();
            
            for (int i = 0; i < hash.Length; i++)
            {
                Data.Append($"{hash[i]:X2}");
                if ((i % 4) == 3)
                {
                    Data.Append(" ");
                }
            }
            
            return Data.ToString();
        }
    }
}