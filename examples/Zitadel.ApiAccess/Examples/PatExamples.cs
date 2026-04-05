using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;

using Zitadel.Abstractions.Credentials;
using Zitadel.AccessTokenManagement.Extensions;
using Zitadel.Auth.V1;

namespace Zitadel.ApiAccess.Examples;

public static class PatExamples
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// Retrieves information about the current user using the Personal Access Token (PAT) authentication mechanism.
    /// This is considered the least safe option, use a PAT token when you have no other choice.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task GetMyUser()
    {
        var services = new ServiceCollection();
        services.ConfigureZitadelServiceAccount(c =>
        {
            c.Pat = new ZitadelServiceAccount.PatProfile
            {
                AccessToken = "myaccesstoken"
            };
        });

        services
            .AddGrpcClient<AuthService.AuthServiceClient>(c => c.Address = new Uri("https://zitadel-instance-abcdef.zitadel.cloud/"))
            .WithZitadelServiceAccount();

        var sp = services.BuildServiceProvider();
        var client = sp.GetRequiredService<AuthService.AuthServiceClient>();
        var result = await client.GetMyUserAsync(new GetMyUserRequest());

        Console.WriteLine($"""
                           ========== PAT TOKEN RESULT ================= 
                           {JsonSerializer.Serialize(result.User, SerializerOptions)}
                           """);
        Console.WriteLine();
    }
}

