using Zitadel.Abstractions.Delegates;

namespace Zitadel.Abstractions.Credentials;

/// <summary>
/// Application for ZITADEL. An application is an OIDC application type
/// that allows a backend (for example, for some single page application) api to
/// check if sent credentials from a client are valid or not.
/// </summary>
public record ZitadelApplication
{
    /// <summary>
    /// Configures the application to use client assertion (JWT) for token introspection. The mechanism is defined here:
    /// <a href="https://zitadel.com/docs/guides/integrate/token-introspection/private-key-jwt">Client assertion (JWT) authentication</a>.
    /// </summary>
    public JwtProfile? Jwt { get; set; }

    /// <summary>
    /// The JWT profile can be configured when setting up an api application in zitadel.
    /// </summary>
    public class JwtProfile
    {
        /// <summary>
        /// The application id, this can be found in the downloaded JSON key.
        /// </summary>
        public string AppId { get; set; } = string.Empty;

        /// <summary>
        /// The key id, this can be found in the downloaded JSON key.
        /// </summary>
        public string KeyId { get; set; } = string.Empty;

        /// <summary>
        /// The actual key, this can be found in the downloaded JSON key. This is mandatory when no SignJwt delegate has
        /// been configured.
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// A delegate that is used to offload the signing of a JWT to an external vault. Useful when the private key
        /// is not allowed to be saved on a developer machine.
        /// </summary>
        public SignJwtAsync? SignJwtAsync { get; set; }
    }
}
