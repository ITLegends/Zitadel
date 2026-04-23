using Duende.AspNetCore.Authentication.OAuth2Introspection;

using Zitadel.Abstractions.Credentials;

namespace Zitadel.AspNetCore.Authentication.Options;

/// <summary>
/// Public options for ZITADEL introspection handler.
/// </summary>
public class ZitadelIntrospectionOptions : OAuth2IntrospectionOptions
{
    /// <summary>
    /// <para>
    /// Alternative to HTTP Basic authentication against the token endpoint.
    /// JWT Profile is recommended instead of using Client ID and Client Secret.
    /// </para>
    /// <para>
    /// If set after the "configure options" function, an event handler to update
    /// the client assertion is added.
    /// </para>
    /// </summary>
    public ZitadelApplication.JwtProfile? Jwt { get; set; }
}
