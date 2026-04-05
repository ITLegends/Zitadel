namespace Zitadel.Abstractions;

/// <summary>
/// Contains reserved OpenID Connect scopes defined by ZITADEL.
/// These scopes can be used to request additional claims in tokens.
/// <see href="https://zitadel.com/docs/apis/openidoauth/scopes#reserved-scopes">Documentation</see>.
/// </summary>
public static class ZitadelScopes
{
    /// <summary>
    /// By using this scope a client can request the claim urn:zitadel:iam:org:project:{projectid}:roles to be asserted
    /// for each requested project. All projects of the token audience, requested by the
    /// urn:zitadel:iam:org:project:id:{projectid}:aud scopes will be used.
    /// </summary>
    public const string ProjectRoles = "urn:zitadel:iam:org:projects:roles";

    /// <summary>
    /// By adding this scope: id, name and primary_domain of the user's organization will be included in the token.
    /// </summary>
    public const string UserResourceOwner = "urn:zitadel:iam:user:resourceowner";

    /// <summary>
    /// By adding this scope, the ZITADEL project id will be added to the audience of the access token.
    /// </summary>
    public const string ZitadelProject = "urn:zitadel:iam:org:project:id:zitadel:aud";

    /// <summary>
    /// By adding this scope, the metadata of the user will be included in the token. The values are base64 encoded.
    /// </summary>
    public const string UserMetadata = "urn:zitadel:iam:user:metadata";

    /// <summary>
    /// By using this scope a client can request the claim urn:zitadel:iam:org:project:roles to be asserted when possible.
    /// As an alternative approach, you can enable all roles to be asserted from the project a client belongs to.
    /// <example>urn:zitadel:iam:org:project:role:user</example>
    /// </summary>
    /// <param name="roleKey">the role key to include in the scope.</param>
    /// <returns>The constructed scope with the provided role key.</returns>
    public static string ProjectRole(string roleKey) => $"urn:zitadel:iam:org:project:role:{roleKey}";

    /// <summary>
    /// When requesting this scope ZITADEL will enforce that the user is a member of the selected organization.
    /// If the organization does not exist a failure is displayed. It will assert the urn:zitadel:iam:user:resourceowner
    /// claims.
    /// <example>urn:zitadel:iam:org:id:178204173316174381</example>
    /// </summary>
    /// <param name="organizationId">The organization id to construct the scope with.</param>
    /// <returns>The constructed scope with the provided organization id.</returns>
    public static string Organization(string organizationId) => $"urn:zitadel:iam:org:id:{organizationId}";

    /// <summary>
    /// When requesting this scope ZITADEL will enforce that the user is a member of the selected organization and the
    /// username is suffixed by the provided domain. If the organization does not exist a failure is displayed.
    /// <example>urn:zitadel:iam:org:domain:primary:acme.ch</example>
    /// </summary>
    /// <param name="domainName">The domain name to construct the scope with.</param>
    /// <returns>The constructed scope with the provided domain name.</returns>
    public static string Domain(string domainName) => $"urn:zitadel:iam:org:domain:primary:{domainName}";

    /// <summary>
    /// This scope can be used one or more times to limit the granted organization IDs in the returned roles.
    /// Unknown organization IDs are ignored. When this scope is not used, all granted organizations are returned inside
    /// the roles.
    /// <example>urn:zitadel:iam:org:roles:id:178204173316174381</example>
    /// </summary>
    /// <param name="organizationId">The organization id to construct the scope with.</param>
    /// <returns>THe constructed scope with the provided organization id.</returns>
    public static string OrganizationRoles(string organizationId) => $"urn:zitadel:iam:org:roles:id:{organizationId}";

    /// <summary>
    /// By adding this scope, the requested project id will be added to the audience of the access token.
    /// <example>urn:zitadel:iam:org:project:id:69234237810729019:aud</example>
    /// </summary>
    /// <param name="projectId">The project id to construct the scope with.</param>
    /// <returns>The constructed scope with the project id.</returns>
    public static string ProjectAudience(string projectId) => $"urn:zitadel:iam:org:project:id:{projectId}:aud";

    /// <summary>
    /// By adding this scope the user will directly be redirected to the identity provider to authenticate.
    /// Make sure you also send the Organization Domain scope if a custom login policy is configured.
    /// Otherwise, the system will not be able to identify the identity provider.
    /// <example>urn:zitadel:iam:org:idp:id:76625965177954913</example>
    /// </summary>
    /// <param name="identityProviderId">The identity provider id to construct the scope with.</param>
    /// <returns>The constructed scope with the provided identity provider id.</returns>
    public static string IdentityProvider(string identityProviderId) => $"urn:zitadel:iam:org:idp:id:{identityProviderId}";
}
