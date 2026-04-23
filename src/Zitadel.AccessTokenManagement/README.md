# IT.Legends.Zitadel.AccessTokenManagement

A library that aims to make accessing [ZITADEL](https://zitadel.com) APIs easy. Provides automatic service account token management — so your application can call ZITADEL APIs or other ZITADEL-protected APIs without managing tokens by hand.

[![NuGet](https://img.shields.io/nuget/v/IT.Legends.Zitadel.AccessTokenManagement.svg)](https://www.nuget.org/packages/IT.Legends.Zitadel.AccessTokenManagement)
[![NuGet Downloads](https://img.shields.io/nuget/dt/IT.Legends.Zitadel.AccessTokenManagement.svg)](https://www.nuget.org/packages/IT.Legends.Zitadel.AccessTokenManagement)

---

## Why this package?

- **Minimal dependencies** - only `Duende.AccessTokenManagement`.
- **Batteries-included token management** - tokens are acquired, cached, and refreshed automatically via `Duende.AccessTokenManagement`.
- **Supports external JWT signing** — the private key never has to touch your application. Signing can be offloaded to an external key vault (e.g. Azure Key Vault).
- **Named accounts** — configure multiple independent service accounts in one application for multi-tenant or multi-API scenarios.

---

## Installation

```shell
dotnet add package IT.Legends.Zitadel.AccessTokenManagement
```

---

## Service Account Authentication

A ZITADEL service account is a non-human identity used to call the ZITADEL API or any API protected by ZITADEL. Three authentication methods are supported — configure exactly one per service account.

| Method                      | When to use                                    |
|-----------------------------|------------------------------------------------|
| Personal Access Token (PAT) | Simple scripts, local tooling, static tokens   |
| Client Credentials          | Standard OAuth2 client_id + client_secret flow |
| JWT Profile                 | Asymmetric key, no secret over the wire        |

For examples please check out the examples directory.

---

## Setup

Call `ConfigureZitadelServiceAccount()` once during startup to register the service account, then attach it to any `IHttpClientBuilder` (including gRPC clients) with `WithZitadelServiceAccount()`.

### Personal Access Token

```csharp
builder.Services.ConfigureZitadelServiceAccount(o =>
{
    o.Pat = new ZitadelServiceAccount.PatProfile
    {
        AccessToken = "<your-personal-access-token>",
    };
});
```

### Client Credentials

```csharp
builder.Services.ConfigureZitadelServiceAccount(o =>
{
    o.Authority = "https://<your-instance>.zitadel.cloud";
    o.Scopes = ["openid", "urn:zitadel:iam:org:project:id:<project-id>:aud"];
    o.ClientCredentials = new ZitadelServiceAccount.ClientCredentialsProfile
    {
        ClientId = "<client-id>",
        ClientSecret = "<client-secret>",
    };
});
```

### JWT Profile (Recommended)

```csharp
builder.Services.ConfigureZitadelServiceAccount(o =>
{
    o.Authority = "https://<your-instance>.zitadel.cloud";
    o.Scopes = ["openid", "urn:zitadel:iam:org:project:id:<project-id>:aud"];
    o.Jwt = new ZitadelServiceAccount.JwtProfile
    {
        UserId = "<service-account-user-id>",
        KeyId = "<key-id>",
        // Provide either a static private key...
        Key = "-----BEGIN RSA PRIVATE KEY-----\nMY SUPER SECRET KEY\n-----END RSA PRIVATE KEY-----\n",
        // ...or offload signing to an external vault (e.g. Azure Key Vault)
        SignJwtAsync = async (services, data, ct) =>
        {
            var keyId = "https://my-vault.vault.azure.net/keys/zitadel-sa/123456789012345678";
            var client = new CryptographyClient(new Uri(keyId), new DefaultAzureCredential());
            return (await client.SignDataAsync(SignatureAlgorithm.RS256, data, ct)).Signature;
        },
    };
});
```

The JWT credentials JSON can be downloaded from the ZITADEL console when you create a service account key.

Inject and use the client directly — no manual token handling required:

```csharp
public class UserSyncService(UserService.UserServiceClient users)
{
    public async Task<GetUserByIDResponse> GetAsync(string userId, CancellationToken ct)
        => await users.GetUserByIDAsync(new GetUserByIDRequest { UserId = userId }, cancellationToken: ct);
}
```

The full list of available clients follows ZITADEL's API versioning — see the [ZITADEL API Reference](https://zitadel.com/docs/apis/introduction) for what each service exposes.

---

## Calling ZITADEL-Protected APIs

The service account token management is not limited to ZITADEL's own APIs. Any `HttpClient` can be wired up the same way:

```csharp
builder.Services
    .ConfigureZitadelServiceAccount(o => { /* ... */ })
    .AddHttpClient<MyApiClient>(o =>
    {
        o.BaseAddress = new Uri("https://my-internal-api.example.com");
    })
    .WithZitadelServiceAccount();
```

---

## Calling ZITADEL APIs

Add the `IT.Legends.Zitadel.Clients` package and register the client you need with `AddGrpcClient<T>()`, then attach the service account with `WithZitadelServiceAccount()`. Token acquisition and refresh happen automatically.

```csharp
builder.Services
    .ConfigureZitadelServiceAccount(o => { /* ... */ })
    .AddGrpcClient<UserService.UserServiceClient>(o =>
    {
        o.Address = new Uri("https://<your-instance>.zitadel.cloud");
    })
    .WithZitadelServiceAccount();
```

---

## Multiple Service Accounts

Use named accounts when your application needs to authenticate as different identities — for instance, one account per downstream API or organization:

```csharp
builder.Services
    .ConfigureZitadelServiceAccount("billing-api", o => { /* ... */ })
    .ConfigureZitadelServiceAccount("reporting-api", o => { /* ... */ });

builder.Services
    .AddGrpcClient<BillingService.BillingServiceClient>(o => { /* ... */ })
    .WithZitadelServiceAccount("billing-api");

builder.Services
    .AddHttpClient<ReportingClient>(o => { /* ... */ })
    .WithZitadelServiceAccount("reporting-api");
```

---

## Configuration Reference

### `ZitadelServiceAccount`

| Property            | Type                        | Description                                                                    |
|---------------------|-----------------------------|--------------------------------------------------------------------------------|
| `Authority`         | `string`                    | Full URL of the authority that issues the tokens (not required when using PAT) |
| `TokenPath`         | `string`                    | Path to retrieve the token relative to the authority.                          |
| `Scopes`            | `string[]`                  | Scopes to request. Defaults to `["openid"]`                                    |
| `Pat`               | `PatProfile?`               | Personal Access Token configuration                                            |
| `ClientCredentials` | `ClientCredentialsProfile?` | Client credentials configuration                                               |
| `Jwt`               | `JwtProfile?`               | JWT Profile configuration (recommended)                                        |

Exactly one of `Pat`, `ClientCredentials`, or `Jwt` must be set — startup validation will fail otherwise.

### `ZitadelServiceAccount.JwtProfile`

| Property       | Type            | Description                                                        |
|----------------|-----------------|--------------------------------------------------------------------|
| `UserId`       | `string`        | Service account user ID from the downloaded key JSON               |
| `KeyId`        | `string`        | Key ID from the downloaded key JSON                                |
| `Key`          | `string?`       | PEM-encoded RSA private key. Required unless `SignJwtAsync` is set |
| `SignJwtAsync` | `SignJwtAsync?` | Delegate to offload signing to an external vault                   |

---

## Related Packages

| Package                                                               | Purpose                                                                         |
|-----------------------------------------------------------------------|---------------------------------------------------------------------------------|
| [IT.Legends.Zitadel.Abstractions](../Zitadel.Abstractions/README.md)  | Shared types, claim constants, and credential models — no external dependencies |
| [IT.Legends.Zitadel.AspNetCore](../Zitadel.AspNetCore/README.md)      | ASP.NET Core authentication middleware (OIDC, OAuth2 introspection, fake auth)  |
| [IT.Legends.Zitadel.Clients](../Zitadel.Clients/README.md)            | Generated gRPC clients to access the ZITADEL APIs                               |
