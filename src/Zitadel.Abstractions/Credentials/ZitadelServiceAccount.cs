using Zitadel.Abstractions.Delegates;

namespace Zitadel.Abstractions.Credentials;

/// <summary>
/// A ZITADEL service account is a non-human account to access the Zitadel API or another API that
/// is protected by ZITADEL.
/// </summary>
public class ZitadelServiceAccount
{
    /// <summary>
    /// The token endpoint to retrieve tokens from.
    /// </summary>
    public string TokenEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// The scopes that should be passed to ZITADEL on authentication.
    /// </summary>
    public string[] Scopes { get; set; } = ["openid", "profile"];

    /// <summary>
    /// Configures the service account with a Personal Access Token. The mechanism is defined here:
    /// <a href="https://zitadel.com/docs/guides/integrate/service-accounts/personal-access-token">Personal access token authentication</a>.
    /// </summary>
    public PatProfile? Pat { get; set; }

    /// <summary>
    /// Configures the service account to use the client credentials flow. The mechanism is defined here
    /// <a href="https://zitadel.com/docs/guides/integrate/service-accounts/client-credentials">Client credentials authentication</a>.
    /// </summary>
    public ClientCredentialsProfile? ClientCredentials { get; set; }

    /// <summary>
    /// Configures the service account to use the client assertion flow (JWT). The mechanism is defined here:
    /// <a href="https://zitadel.com/docs/guides/integrate/service-accounts/private-key-jwt">Client assertion (JWT) authentication</a>.
    /// </summary>
    public JwtProfile? Jwt { get; set; }

    /// <summary>
    /// Configuration for the service account to use the Personal Access Token flow.
    /// </summary>
    public class PatProfile
    {
        /// <summary>
        /// The static access token to send on every request.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configuration for the service account to use the client credentials basic authentication flow.
    /// </summary>
    public class ClientCredentialsProfile
    {
        /// <summary>
        /// The clientId that belongs to the service account. This is usually the service account name.
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// The client secret that belongs to this service account.
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configuration for the service account to use the client assertion flow (JWT)
    /// </summary>
    public class JwtProfile
    {
        /// <summary>
        /// The user id in ZITADEL, this can be found in the downloaded JSON key.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

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
