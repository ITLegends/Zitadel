using Duende.AccessTokenManagement;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Zitadel.Abstractions.Credentials;
using Zitadel.AccessTokenManagement.ClientAssertion;
using Zitadel.AccessTokenManagement.Constants;
using Zitadel.AccessTokenManagement.Options;

namespace Zitadel.AccessTokenManagement.Extensions;

/// <summary>
/// Provides extension methods for configuring <see cref="IServiceCollection"/> 
/// to automatically attach access tokens obtained from Zitadel service accounts.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures the default Zitadel service account with the specified options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service account configuration to.</param>
    /// <param name="configureOptions">A callback to configure the <see cref="ZitadelServiceAccount"/> object.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection ConfigureZitadelServiceAccount(
        this IServiceCollection services,
        Action<ZitadelServiceAccount> configureOptions)
        => services.ConfigureZitadelServiceAccount(ZitadelClientConstants.DefaultServiceAccountName, configureOptions);

    /// <summary>
    /// Configures a named Zitadel service account with the specified options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service account configuration to.</param>
    /// <param name="serviceAccountName">The name of the service account being configured.</param>
    /// <param name="configureOptions">A callback to configure the <see cref="ZitadelServiceAccount"/> object.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance with the service account configuration added.</returns>
    public static IServiceCollection ConfigureZitadelServiceAccount(
        this IServiceCollection services,
        string serviceAccountName,
        Action<ZitadelServiceAccount> configureOptions)
    {
        services.Configure(serviceAccountName, configureOptions);
        services.TryAddSingleton<IValidateOptions<ZitadelServiceAccount>, ZitadelServiceAccountValidator>();
        services.TryAddSingleton<IClientAssertionService, ZitadelClientAssertionService>();
        services.TryAddSingleton<ZitadelClientAssertionMessageHandler>();
        services.ConfigureOptions<ConfigureClientCredentialsClient>();
        services.AddClientCredentialsTokenManagement();

        services
            .AddHttpClient($"{serviceAccountName}-backchannel")
            .AddHttpMessageHandler<ZitadelClientAssertionMessageHandler>();

        return services;
    }
}
