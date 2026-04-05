namespace Zitadel.Abstractions;

/// <summary>
/// Reserved claim types used by ZITADEL.
/// </summary>
public static class ZitadelClaimTypes
{
    /// <summary>
    /// When roles are asserted, ZITADEL does this by providing the <c>id</c> and
    /// <c>primaryDomain</c> below the role.
    /// </summary>
    public const string ProjectRoles = "urn:zitadel:iam:org:project:roles";

    /// <summary>
    /// This claim represents the Organization Domain the user belongs to.
    /// </summary>
    public const string PrimaryDomain = "urn:zitadel:iam:org:domain:primary";

    /// <summary>
    /// The metadata claim will include all metadata of a user. The values are base64 encoded.
    /// </summary>
    public const string UserMetadata = "urn:zitadel:iam:user:metadata";

    /// <summary>
    /// This claim represents the user's organization ID.
    /// </summary>
    public const string ResourceOwnerId = "urn:zitadel:iam:user:resourceowner:id";

    /// <summary>
    /// This claim represents the user's organization's name.
    /// </summary>
    public const string ResourceOwnerName = "urn:zitadel:iam:user:resourceowner:name";

    /// <summary>
    /// This claim represents the user's Organization Domain.
    /// </summary>
    public const string ResourceOwnerPrimaryDomain = "urn:zitadel:iam:user:resourceowner:primary_domain";

    /// <summary>
    /// This claim is set during Actions as a log, e.g., if two custom claims with the same keys are set.
    /// </summary>
    /// <param name="actionName">The name of the action.</param>
    /// <returns>The claim type for the action log.</returns>
    public static string ActionLog(string actionName) => $"urn:zitadel:iam:action:{actionName}:log";

    /// <summary>
    /// This claim represents the Organization Domain the user belongs to, scoped to a specific domain.
    /// </summary>
    /// <param name="domainName">The primary domain name.</param>
    /// <returns>The claim type for the scoped primary domain.</returns>
    public static string PrimaryDomainScoped(string domainName) => $"urn:zitadel:iam:org:domain:primary:{domainName}";

    /// <summary>
    /// When roles are asserted, ZITADEL does this by providing the <c>id</c> and
    /// <c>primaryDomain</c> below the role, scoped to a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>The claim type for project-scoped roles.</returns>
    public static string ProjectRolesForProject(string projectId) => $"urn:zitadel:iam:org:project:{projectId}:roles";

    /// <summary>
    /// Constructor for organization-specific role claims.
    /// They are used to specify roles in a specific organization.
    /// Check for those roles with the policies added with.
    /// </summary>
    /// <remarks>This claim type is not defined by Zitadel.</remarks>
    /// <param name="orgId">The id of the organization.</param>
    /// <returns>A role name.</returns>
    public static string OrganizationRole(string orgId) => $"urn:zitadel:iam:org:{orgId}:project:roles";
}
