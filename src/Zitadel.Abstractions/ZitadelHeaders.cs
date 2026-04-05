namespace Zitadel.Abstractions;

/// <summary>
/// Contains headers used when requesting data from the ZITADEL Api.
/// </summary>
public static class ZitadelHeaders
{
    /// <summary>
    /// Header used to provide the organization context to grpc/rest api calls.
    /// </summary>
    public const string Organization = "x-zitadel-orgid";
}
