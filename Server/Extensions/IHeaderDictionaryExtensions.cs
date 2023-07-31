using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using Remotely.Server.Auth;

namespace Remotely.Server.Extensions;

public static class IHeaderDictionaryExtensions
{
    /// <summary>
    /// If true, ensures that the user was authorized via <see cref="ApiAuthorizationFilter"/>,
    /// and is either an Administrator or is using a valid API access token.
    /// </summary>
    /// <param name="headers"></param>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    public static bool TryGetOrganizationId(
        this IHeaderDictionary headers, 
        [NotNullWhen(true)] out string? organizationId)
    {
        organizationId = null;

        if (!headers.TryGetValue("OrganizationId", out var orgId))
        {
            return false;
        }

        organizationId = $"{orgId}";
        
        if (string.IsNullOrWhiteSpace(organizationId))
        {
            return false;
        }

        return true;
    }
}
