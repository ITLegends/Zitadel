using Microsoft.AspNetCore.Authentication;

namespace Zitadel.AspNetCore.Authentication.Options;

internal class LocalFakeZitadelSchemeOptions : AuthenticationSchemeOptions
{
    public LocalFakeZitadelOptions FakeZitadelOptions { get; set; } = new();
}
