using System.Security.Cryptography;

namespace Immense.RemoteControl.Shared.Helpers;

public class RandomGenerator
{
    private const string AllowableCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateString(int length)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        return new string(bytes.Select(x => AllowableCharacters[x % AllowableCharacters.Length]).ToArray());
    }

    public static string GenerateAccessKey()
    {
        return GenerateString(64);
    }
}
