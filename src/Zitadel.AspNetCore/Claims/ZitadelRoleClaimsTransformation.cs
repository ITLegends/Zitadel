using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;

using Zitadel.Abstractions;

namespace Zitadel.AspNetCore.Claims;

internal sealed class ZitadelRoleClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.HasClaim(c => c.Type == ClaimTypes.Role)) return Task.FromResult(principal);

        var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ZitadelClaimTypes.ProjectRoles);
        if (roleClaim is null) return Task.FromResult(principal);

        var roleIdentity = new ClaimsIdentity();
        var claims = ZitadelRoleClaimsParser.Parse(roleClaim.Value);
        roleIdentity.AddClaims(claims);

        principal.AddIdentity(roleIdentity);
        return Task.FromResult(principal);
    }
}
