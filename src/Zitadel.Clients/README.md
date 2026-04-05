# IT.Legends.Zitadel.Clients

gRPC client library for the [ZITADEL](https://zitadel.com) API. Contains gRPC clients automatically generated from the [ZITADEL proto files](https://github.com/zitadel/zitadel/tree/main/proto) — one client per ZITADEL service.

[![NuGet](https://img.shields.io/nuget/v/IT.Legends.Zitadel.Clients.svg)](https://www.nuget.org/packages/IT.Legends.Zitadel.Clients)
[![NuGet Downloads](https://img.shields.io/nuget/dt/IT.Legends.Zitadel.Clients.svg)](https://www.nuget.org/packages/IT.Legends.Zitadel.Clients)

---

## Installation

```shell
dotnet add package IT.Legends.Zitadel.Clients
```

---

## Related Packages

| Package                                                                                | Purpose                                                                         |
|----------------------------------------------------------------------------------------|---------------------------------------------------------------------------------|
| [IT.Legends.Zitadel.Abstractions](../Zitadel.Abstractions/README.md)                   | Shared types, claim constants, and credential models — no external dependencies |
| [IT.Legends.Zitadel.AccessTokenManagement](../Zitadel.AccessTokenManagement/README.md) | Provides automatic service account token management                    |
| [IT.Legends.Zitadel.AspNetCore](../Zitadel.AspNetCore/README.md)                       | ASP.NET Core authentication middleware (OIDC, OAuth2 introspection, fake auth)  |
