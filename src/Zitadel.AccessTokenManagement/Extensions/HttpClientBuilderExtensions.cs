using Duende.AccessTokenManagement;

using Microsoft.Extensions.DependencyInjection;

using Zitadel.AccessTokenManagement.Constants;
using Zitadel.AccessTokenManagement.TokenRetriever;

namespace Zitadel.AccessTokenManagement.Extensions;

/// <summary>
/// Provides extension methods for configuring <see cref="IHttpClientBuilder"/> instances
/// to automatically attach access tokens obtained from Zitadel service accounts.
/// </summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>
    /// Configures an <see cref="IHttpClientBuilder"/> to use the default Zitadel service account
    /// for authentication.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IHttpClientBuilder"/> to configure.
    /// </param>
    /// <returns>
    /// The configured <see cref="IHttpClientBuilder"/> instance with the default Zitadel service account.
    /// </returns>
    public static IHttpClientBuilder WithZitadelServiceAccount(this IHttpClientBuilder builder)
        => builder.WithZitadelServiceAccount(ZitadelClientConstants.DefaultServiceAccountName);

    /// <summary>
    /// Configures an <see cref="IHttpClientBuilder"/> to use a specified Zitadel service account
    /// for authentication.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IHttpClientBuilder"/> to configure.
    /// </param>
    /// <param name="serviceAccountName">
    /// The name of the Zitadel service account to use for authentication.
    /// </param>
    /// <returns>
    /// The configured <see cref="IHttpClientBuilder"/> instance.
    /// </returns>
    public static IHttpClientBuilder WithZitadelServiceAccount(
        this IHttpClientBuilder builder,
        string serviceAccountName)
    {
        builder.AddHttpMessageHandler(provider => provider.GetZitadelTokenRequestHandler(serviceAccountName));
        return builder;
    }

    private static AccessTokenRequestHandler GetZitadelTokenRequestHandler(this IServiceProvider provider, string serviceAccountName)
    {
        var clientName = ClientCredentialsClientName.Parse(serviceAccountName);
        var tokenRetriever = ActivatorUtilities.CreateInstance<ZitadelServiceAccountTokenRetriever>(provider, clientName);

        return ActivatorUtilities.CreateInstance<AccessTokenRequestHandler>(provider, tokenRetriever);
    }
}
