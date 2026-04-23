# IT.Legends.Zitadel.AspNetCore

ASP.NET Core authentication middleware for [ZITADEL](https://zitadel.com) — the open-source identity platform. Supports OpenID Connect for web applications, OAuth2 token introspection for APIs, and a local fake handler for frictionless development and testing.

[![NuGet](https://img.shields.io/nuget/v/IT.Legends.Zitadel.AspNetCore.svg)](https://www.nuget.org/packages/IT.Legends.Zitadel.AspNetCore)
[![NuGet Downloads](https://img.shields.io/nuget/dt/IT.Legends.Zitadel.AspNetCore.svg)](https://www.nuget.org/packages/IT.Legends.Zitadel.AspNetCore)

---

## Why this package?

- **Minimal dependencies** - only `Microsoft.AspNetCore.Authentication.OpenIdConnect` and `Duende.AspNetCore.Authentication.OAuth2Introspection`.
- **First-class developer experience** - the fake authentication handler means you never need a running ZITADEL instance to build or test locally.
- **Supports external signing of JWT** - For maximum security the signing of JSON web tokens can be offloaded to an external key vault.

---

## Installation

```shell
dotnet add package IT.Legends.Zitadel.AspNetCore
```

---

## Scenarios

| Scenario                               | Method                      |
|----------------------------------------|-----------------------------|
| Web app (server-side, MVC/Razor Pages) | `AddZitadel()`              |
| API protected by access token          | `AddZitadelIntrospection()` |
| Local development / testing            | `AddZitadelFake()`          |


For examples please check out the examples directory.

---

## Web Application (OpenID Connect)

Use `AddZitadel()` when your application renders server-side pages and needs to establish a user session. Under the hood this configures the standard OpenID Connect handler with PKCE, userinfo endpoint claim mapping, and ZITADEL's role structure.

```csharp
builder.Services
    .AddAuthorization()
    .AddAuthentication(ZitadelDefaults.AuthenticationScheme)
    .AddZitadel(o =>
    {
        o.Authority = "https://<your-instance>.zitadel.cloud";
        o.ClientId = "<your-client-id>";
        o.SignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddExternalCookie()
    .Configure(o =>
    {
        o.Cookie.HttpOnly = true;
        o.Cookie.IsEssential = true;
        o.Cookie.SameSite = SameSiteMode.None;
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
```

The handler automatically maps standard OIDC claims (`sub`, `name`, `email`, `given_name`, `family_name`, …) and parses ZITADEL's project-scoped role structure into individual role claims so that `User.IsInRole()` works out of the box.

---

## Web API (OAuth2 Token Introspection)

Use `AddZitadelIntrospection()` when your API needs to validate bearer tokens presented by callers. The introspection endpoint handles both JWT and opaque tokens — no local key management required.

ZITADEL supports two client authentication methods for the introspection endpoint.

### Basic Authentication

```csharp
builder.Services
    .AddAuthorization()
    .AddAuthentication()
    .AddZitadelIntrospection(o =>
    {
        o.Authority = "https://<your-instance>.zitadel.cloud";
        o.ClientId = "<your-client-id>";
        o.ClientSecret = "<your-client-secret>";
    });
```

### JWT Profile (Recommended)

JWT Profile eliminates the need for a shared secret. Instead, your API uses an asymmetric key pair to authenticate — much harder to exfiltrate and easier to rotate.

```csharp
builder.Services
    .AddAuthorization()
    .AddAuthentication()
    .AddZitadelIntrospection(o =>
    {
        o.Authority = "https://<your-instance>.zitadel.cloud";
        o.Jwt = ZitadelApplication.JwtProfile
        {
            AppId = <appId>,
            KeyId = <KeyId>,
            // Provide either a static private key
            Key = "-----BEGIN RSA PRIVATE KEY-----\nMY SUPER SECRET KEY\n-----END RSA PRIVATE KEY-----\n"
            // Or offload the signing of the JWT to an external vault (e.g. azure key vault)
            SignJwtAsync = async (_, data, ct) =>
            {
                var keyId = "https://my-vault.vault.azure.net/keys/zitadel-application/123456789012345678";
                var cryptoClient = new CryptographyClient(new Uri(keyId), new AzureCliCredential());
                return (await cryptoClient.SignDataAsync(SignatureAlgorithm.RS256, data, ct)).Signature;
            }
            
        };
    });
```

The application credentials JSON can be downloaded from the ZITADEL console when you create or configure an API application with JWT Profile authentication.

### Token Introspection Caching

Introspecting every request adds latency and load on your ZITADEL instance. Caching is built-in using by using Duende's oauth2introspection client.

---

## Local Development / Fake Authentication

The fake authentication handler lets you develop and test without a running ZITADEL instance. All requests are authenticated by default. The handler injects a configurable identity so you can exercise authorization logic as if a real user is present.

```csharp
builder.Services
    .AddAuthorization()
    .AddAuthentication()
    .AddZitadelFake(o =>
    {
        o.FakeZitadelId = "user-1234";
        o.Roles = ["admin", "editor"];
        o.AdditionalClaims =
        [
            new Claim(ClaimTypes.Email, "dev@example.com"),
            new Claim(ClaimTypes.Name, "Dev User"),
        ];
    });
```

You can also use the fluent `AddClaim` helper:

```csharp
.AddZitadelFake(o =>
{
    o.FakeZitadelId = "user-1234";
    o.AddClaim(ClaimTypes.Email, "dev@example.com")
     .AddClaim(ClaimTypes.Name, "Dev User");
});
```

### Per-Request Overrides

Two request headers let you override the fake handler's behaviour at runtime — useful for integration tests that need to exercise both authenticated and unauthenticated code paths without reconfiguring the handler:

| Header                         | Effect                                                   |
|--------------------------------|----------------------------------------------------------|
| `x-zitadel-fake-auth: false`   | The request is treated as unauthenticated                |
| `x-zitadel-fake-user-id: <id>` | Replaces the configured `FakeZitadelId` for this request |

---

## Configuration Reference

### `ZitadelIntrospectionOptions`

Extends `OAuth2IntrospectionOptions` from `Duende.AspNetCore.Authentication.OAuth2Introspection`.

| Property       | Type                             | Description                                           |
|----------------|----------------------------------|-------------------------------------------------------|
| `Authority`    | `string`                         | Base URL of your ZITADEL instance                     |
| `ClientId`     | `string`                         | Client ID for Basic Auth introspection                |
| `ClientSecret` | `string`                         | Client secret for Basic Auth introspection            |
| `Jwt`          | `ZitadelApplication.JwtProfile?` | JWT Profile credentials (recommended over Basic Auth) |

### `LocalFakeZitadelOptions`

| Property           | Type                     | Description                                           |
|--------------------|--------------------------|-------------------------------------------------------|
| `FakeZitadelId`    | `string`                 | The `sub` claim value injected into the fake identity |
| `Roles`            | `IEnumerable<string>`    | Roles attached to the fake identity                   |
| `AdditionalClaims` | `IList<Claim>`           | Extra claims to include                               |
| `Events`           | `LocalFakeZitadelEvents` | Hooks for dynamic per-request identity mutation       |

---

## Related Packages

| Package                                                                                | Purpose                                                                         |
|----------------------------------------------------------------------------------------|---------------------------------------------------------------------------------|
| [IT.Legends.Zitadel.Abstractions](../Zitadel.Abstractions/README.md)                   | Shared types, claim constants, and credential models — no external dependencies |
| [IT.Legends.Zitadel.AccessTokenManagement](../Zitadel.AccessTokenManagement/README.md) | Provides automatic service account token management                    |
| [IT.Legends.Zitadel.Clients](../Zitadel.Clients/README.md)                             | Generated gRPC clients to access the ZITADEL APIs                               |
