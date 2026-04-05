using System.Security.Claims;
using System.Text.Json;

using Zitadel.Abstractions;

namespace Zitadel.AspNetCore.Claims;

internal static class ZitadelRoleClaimsParser
{
    public static IEnumerable<Claim> Parse(string jsonRoles)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jsonRoles);
        using var jsonDocument = JsonDocument.Parse(jsonRoles);
        foreach (var claim in Parse(jsonDocument.RootElement))
        {
            yield return claim;
        }
    }

    public static IEnumerable<Claim> Parse(JsonElement jsonElement)
    {
        foreach (var roleKey in jsonElement.EnumerateObject())
        {
            // Regular role
            yield return new Claim(ClaimTypes.Role, roleKey.Name, ClaimValueTypes.String);

            // Role with organization included.
            foreach (var orgKey in roleKey.Value.EnumerateObject())
            {
                yield return new Claim(
                    ZitadelClaimTypes.OrganizationRole(orgKey.Name),
                    roleKey.Name,
                    ClaimValueTypes.String);
            }
        }
    }
}
