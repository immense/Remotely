using System;

namespace Remotely.Server.Models
{
    public class Toast
    {
        public Toast(string guid, string message, string classString, TimeSpan expiration, string styleOverrides)
        {
            Guid = guid;
            Message = message;
            ClassString = classString;
            Expiration = expiration;
            StyleOverrides = styleOverrides;
        }

        public string ClassString { get; }
        public TimeSpan Expiration { get; }
        public string Guid { get; }
        public string Message { get; }
        public string StyleOverrides { get; }
    }
}
