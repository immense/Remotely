using System;
using System.Security.Cryptography;

namespace Remotely.Shared.Utilities
{
    public static class PasswordGenerator
    {
        public static string GeneratePassword(int size)
        {
            var buffer = new byte[size];
            var rng = RandomNumberGenerator.Create();
            rng.GetNonZeroBytes(buffer);
            return Convert.ToBase64String(buffer).Replace("=", "");
        }
    }
}
