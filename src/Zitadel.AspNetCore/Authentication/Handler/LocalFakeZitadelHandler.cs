using System.Security.Claims;
using System.Text.Encodings.Web;

using Duende.IdentityModel;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Zitadel.AspNetCore.Authentication.Events.Context;
using Zitadel.AspNetCore.Authentication.Options;

namespace Zitadel.AspNetCore.Authentication.Handler;

internal sealed class LocalFakeZitadelHandler(
    IOptionsMonitor<LocalFakeZitadelSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<LocalFakeZitadelSchemeOptions>(options, logger, encoder)
{
    private const string FakeAuthHeader = "x-zitadel-fake-auth";
    private const string FakeUserIdHeader = "x-zitadel-fake-user-id";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.Request.Headers.TryGetValue(FakeAuthHeader, out var value) && value == "false")
        {
            return AuthenticateResult.Fail($"The {FakeAuthHeader} was set with value 'false'.");
        }

        var fakeUserIdProvided = Context.Request.Headers.TryGetValue(FakeUserIdHeader, out var providedUserId);
        var userId = fakeUserIdProvided ? providedUserId.ToString() : Options.FakeZitadelOptions.FakeZitadelId;

        var claims = BuildClaims(userId);
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        await Options.FakeZitadelOptions.Events.OnZitadelFakeAuth.Invoke(new LocalFakeZitadelAuthContext(identity));

        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }

    private IEnumerable<Claim> BuildClaims(string userId)
    {
        yield return new Claim(ClaimTypes.NameIdentifier, userId);
        yield return new Claim(JwtClaimTypes.Name, userId);
        yield return new Claim("sub", userId);

        foreach (var additionalClaim in Options.FakeZitadelOptions.AdditionalClaims)
        {
            yield return additionalClaim;
        }

        foreach (var role in Options.FakeZitadelOptions.Roles)
        {
            yield return new Claim(ClaimTypes.Role, role);
        }
    }
}
