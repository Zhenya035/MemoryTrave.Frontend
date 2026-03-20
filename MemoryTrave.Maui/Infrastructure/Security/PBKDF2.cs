using System.Security.Cryptography;
using MemoryTrave.Maui.Models.Security;

namespace MemoryTrave.Maui.Infrastructure.Security;

public class Pbkdf2
{
    private const int Iterations = 600000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public static Pbkdf2Result DeriveKey(string password, byte[]? salt = null)
    {
        salt ??= RandomNumberGenerator.GetBytes(SaltSize);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

        var key = pbkdf2.GetBytes(KeySize);

        return new Pbkdf2Result()
        {
            Key = key,
            Salt = salt
        };
    }
}