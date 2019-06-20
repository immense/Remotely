using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Remotely.Shared.Services
{
    public class RandomGenerator
    {
        private const string allowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

        public string GenerateString(int length)
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
