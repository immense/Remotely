using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared;

public class AppConstants
{
    public const string DefaultProductName = "Remotely";
    public const string DefaultPublisherName = "Immense Networks";
    public const string DebugOrgId = "e8f4ad87-4a4b-4da1-bcb2-1788eaeb80e8";
    public const int EmbeddedDataBlockLength = 256;
    public const long MaxUploadFileSize = 100_000_000;
    public const double ScriptRunExpirationMinutes = 30;
    public const string ApiKeyHeaderName = "X-Api-Key";
    public const string ExpiringTokenHeaderName = "X-Expiring-Token";
}
