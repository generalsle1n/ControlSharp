using System.Security.Cryptography;

namespace ControlSharp.Services;

public class SecretManager
{
    private const int _adminKeySize = 64;

    internal static string CreateAdminToken()
    {
        byte[] TokenData = RandomNumberGenerator.GetBytes(_adminKeySize);
        string Secret = Convert.ToBase64String(TokenData);
        
        return Secret;
    }
}