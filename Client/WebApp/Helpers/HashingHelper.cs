using System.Security.Cryptography;
using System.Text;

namespace WebApp.Helpers;

public class HashingHelper
{
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) return string.Empty;

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}