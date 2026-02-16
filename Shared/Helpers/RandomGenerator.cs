using System.Security.Cryptography;

namespace Remotely.Shared.Helpers;

public class RandomGenerator
{
    private const string AllowableCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ0123456789";
    private const int CharCount = AllowableCharacters.Length;

    public static string GenerateString(int length)
    {
        if (length <= 0)
            return string.Empty;

        Span<char> result = stackalloc char[length];
        Span<byte> randomBytes = stackalloc byte[length * 4];
        
        RandomNumberGenerator.Fill(randomBytes);

        try
        {
            for (int i = 0; i < length; i++)
            {
                uint value = BitConverter.ToUInt32(randomBytes.Slice(i * 4, 4));
                result[i] = AllowableCharacters[(int)(value % CharCount)];
            }
        }
        finally
        {
            CryptographicOperations.ZeroMemory(randomBytes);
        }

        return new string(result);
    }

    public static string GenerateAccessKey()
    {
        return GenerateString(64);
    }
}
