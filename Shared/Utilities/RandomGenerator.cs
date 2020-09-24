using System.Linq;
using System.Security.Cryptography;

namespace Remotely.Shared.Utilities
{
    public class RandomGenerator
    {
        private const string allowableCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ0123456789";

        public static string GenerateString(int length)
        {
            var bytes = new byte[length];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(bytes);
            }

            return new string(bytes.Select(x => allowableCharacters[x % allowableCharacters.Length]).ToArray());
        }
    }
}
