namespace Zitadel.Abstractions;

/// <summary>
/// A set of default values for ZITADEL authentication/authorization.
/// </summary>
public static class ZitadelDefaults
{
    /// <summary>
    /// Default display name.
    /// </summary>
    public const string DisplayName = "ZITADEL";

    /// <summary>
    /// Default display name of the fake handler.
    /// </summary>
    public const string FakeDisplayName = "ZITADEL-Fake";

    /// <summary>
    /// Default authentication scheme name for AddZitadel().
    /// </summary>
    public const string AuthenticationScheme = "ZITADEL";

    /// <summary>
    /// Authentication scheme name for local fake provider.
    /// </summary>
    public const string FakeAuthenticationScheme = "ZITADEL-Fake";

    /// <summary>
    /// Default callback path for local login redirection.
    /// </summary>
    public const string CallbackPath = "/signin-zitadel";
}
