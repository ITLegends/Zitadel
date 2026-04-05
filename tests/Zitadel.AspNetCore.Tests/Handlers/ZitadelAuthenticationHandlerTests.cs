using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;

using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

using Zitadel.Abstractions;
using Zitadel.AspNetCore.Tests.TestHosts;

namespace Zitadel.AspNetCore.Tests.Handlers;

public class ZitadelAuthenticationHandlerTests
{
    [Theory]
    [ClassData(typeof(ZitadelAuthenticationTestHosts))]
    public async Task ShouldBeAbleToCallUnauthorizedEndpoint(IHost host)
    {
        //Arrange
        using var server = host;
        await server.StartAsync();
        var client = server.GetTestClient();

        // Act
        var result = await client.GetFromJsonAsync<ZitadelAuthenticationTestHost.AnonymousResponse>("/unauthed");

        //Assert
        Assert.NotNull(result);
        Assert.Equal("Pong", result.Ping);
    }
    
    [Theory]
    [ClassData(typeof(ZitadelAuthenticationTestHosts))]
    public async Task ShouldReturnForbiddenWhenUserDoesNotHaveRole(IHost host)
    {
        // Arrange
        using var server = host;
        await server.StartAsync();
        var client = server.GetTestClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/admin")
        {
            Headers =
            {
                { HeaderNames.Authorization, $"Bearer {ZitadelAuthenticationTestHost.ViewerToken}" },
                { "x-zitadel-fake-user-id", ZitadelAuthenticationTestHost.ViewerUserId }
            },
        };
        
        // Act
        var result = await client.SendAsync(request);
        
        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }
    
    [Theory]
    [ClassData(typeof(ZitadelAuthenticationTestHosts))]
    public async Task ShouldReturnAuthorizedWhenUsingValidToken(IHost host)
    {
        // Arrange
        using var server = host;
        await server.StartAsync();
        var client = server.GetTestClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/authed")
        {
            Headers = { {  HeaderNames.Authorization, $"Bearer {ZitadelAuthenticationTestHost.ValidToken}" } },
        };
        
        // Act
        var response = await client.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<ZitadelAuthenticationTestHost.AuthenticatedResponse>();
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(ZitadelDefaults.AuthenticationScheme, result.AuthType);
        Assert.Equal(ZitadelAuthenticationTestHost.UserId, result.UserId);
        Assert.Contains(result.Claims, claim => claim.Key == ClaimTypes.Role && ZitadelAuthenticationTestHost.Roles.Contains(claim.Value));
        Assert.Contains(result.Claims, claim => claim .Key == ZitadelAuthenticationTestHost.AdditionalClaim.Type && claim.Value == ZitadelAuthenticationTestHost.AdditionalClaim.Value);
    }
    
    [Theory]
    [ClassData(typeof(ZitadelAuthenticationTestHosts))]
    public async Task ShouldReturnUnauthorizedWhenUsingInvalidToken(IHost host)
    {
        // Arrange
        using var server = host;
        await server.StartAsync();
        var client = server.GetTestClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/authed")
        {
            Headers =
            {
                { HeaderNames.Authorization, $"Bearer {ZitadelAuthenticationTestHost.InvalidToken}" },
                { "x-zitadel-fake-auth", "false" },
            },
        };
        
        // Act
        var response = await client.SendAsync(request);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Theory]
    [ClassData(typeof(ZitadelAuthenticationTestHosts))]
    public async Task ShouldReturnAuthorizedWhenUserHasRole(IHost host)
    {
        // Arrange
        using var server = host;
        await server.StartAsync();
        var client = server.GetTestClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/admin")
        {
            Headers = { {  HeaderNames.Authorization, $"Bearer {ZitadelAuthenticationTestHost.ValidToken}" } },
        };
        
        // Act
        var response = await client.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<ZitadelAuthenticationTestHost.AuthenticatedResponse>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(ZitadelDefaults.AuthenticationScheme, result.AuthType);
        Assert.Equal(ZitadelAuthenticationTestHost.UserId, result.UserId);
        Assert.Contains(result.Claims, claim => claim is { Key: ClaimTypes.Role, Value: "admin" });
    }
}
