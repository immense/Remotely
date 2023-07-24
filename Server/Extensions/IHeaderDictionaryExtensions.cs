using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace Remotely.Server.Extensions;

public static class IHeaderDictionaryExtensions
{
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
