using System.Security.Cryptography;

namespace ControlSharp.Database;

public class SecretManager
{
    private const int _adminKeySize = 64;

    public static string CreateAdminToken()
    {
        byte[] TokenData = RandomNumberGenerator.GetBytes(_adminKeySize);
        string Secret = Convert.ToBase64String(TokenData);
        
        return Secret;
    }
}