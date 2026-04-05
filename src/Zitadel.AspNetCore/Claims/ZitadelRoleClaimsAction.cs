using System.Security.Claims;
using System.Text.Json;

using Microsoft.AspNetCore.Authentication.OAuth.Claims;

using Zitadel.Abstractions;

namespace Zitadel.AspNetCore.Claims;

/// <summary>
/// Claim action to fetch the ZITADEL roles (if provided)
/// from the JWT onto the user identity.
/// </summary>
internal sealed class ZitadelRoleClaimsAction() : ClaimAction(ClaimTypes.Role, ClaimValueTypes.String)
{
    public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
    {
        if (!userData.TryGetProperty(ZitadelClaimTypes.ProjectRoles, out var roles)) return;

        var claims = ZitadelRoleClaimsParser.Parse(roles);
        identity.AddClaims(claims);
    }
}
