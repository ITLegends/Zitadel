namespace Zitadel.AccessTokenManagement.Options;

/// <summary>
/// Used to keep track of registered service accounts.
/// This is used to avoid resolving a service account from the IOptionsMonitor if it is not registered. Resolving a
/// service account that is not registered will always throw an exception due to the service account validator.
/// </summary>
internal class ZitadelServiceAccountRegistry
{
    private readonly HashSet<string> _serviceAccounts = [];

    public void RegisterServiceAccount(string serviceAccount) => _serviceAccounts.Add(serviceAccount);

    public bool IsServiceAccountRegistered(string serviceAccount) => _serviceAccounts.Contains(serviceAccount);
}