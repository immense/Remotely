using Immense.RemoteControl.Shared.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Remotely.Shared.Utilities;
using System;

namespace Remotely.Server.Services;

public interface IOtpProvider
{
    bool Exists(string otp);
    string GetOtp(string deviceId);
    bool OtpMatchesDevice(string otp, string deviceId);
}

public class OtpProvider : IOtpProvider
{
    private static readonly MemoryCache _otpCache = new(new MemoryCacheOptions());

    public bool Exists(string otp)
    {
        return _otpCache.TryGetValue(otp, out _);
    }

    public string GetOtp(string deviceId)
    {
        var otp = RandomGenerator.GenerateString(16);
        _otpCache.Set(otp, deviceId, TimeSpan.FromMinutes(1));
        return otp;
    }

    public bool OtpMatchesDevice(string otp, string deviceId)
    {
        if (_otpCache.TryGetValue(otp, out var cachedItem) &&
            cachedItem is string cachedDevice &&
            cachedDevice == deviceId)
        {
            return true;
        }
        return false;
    }
}
