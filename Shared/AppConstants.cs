using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared
{
    public class AppConstants
    {
        public const string DefaultProductName = "Remotely";
        public const string DefaultPublisherName = "Immense Networks";
        public const int EmbeddedDataBlockLength = 256;
        public const long MaxUploadFileSize = 100_000_000;
        public const double ScriptRunExpirationMinutes = 30;
        public static byte[] EmbeddedImmySignature { get; } = new byte[] { 73, 109, 109, 121, 66, 111, 116, 32, 114, 111, 99, 107, 115, 32, 116, 104, 101, 32, 115, 111, 99, 107, 115, 32, 117, 110, 116, 105, 108, 32, 116, 104, 101, 32, 101, 113, 117, 105, 110, 111, 120, 33 };
    }
}
