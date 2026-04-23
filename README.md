# ZITADEL .NET

[![.NET Release](https://github.com/ITLegends/Zitadel/actions/workflows/release-zitadel.yml/badge.svg)](https://github.com/ITLegends/Zitadel/actions/workflows/release-zitadel.yml)

.NET libraries for integrating with [ZITADEL](https://zitadel.com) — the open-source identity platform. Covers everything from protecting ASP.NET apps and APIs to calling ZITADEL's gRPC management APIs from backend services.

---

## Packages

### [IT.Legends.Zitadel.AspNetCore](./src/Zitadel.AspNetCore/README.md)
[![NuGet](https://img.shields.io/nuget/v/IT.Legends.Zitadel.AspNetCore.svg)](https://www.nuget.org/packages/IT.Legends.Zitadel.AspNetCore)

Authentication middleware for ASP.NET Core. Use this to protect web apps (OpenID Connect) or APIs (OAuth2 token introspection).
Includes a fake authentication handler for local development and testing.

```shell
dotnet add package IT.Legends.Zitadel.AspNetCore
```

### [IT.Legends.Zitadel.AccessTokenManagement](./src/Zitadel.AccessTokenManagement/README.md)
[![NuGet](https://img.shields.io/nuget/v/IT.Legends.Zitadel.AccessTokenManagement.svg)](https://www.nuget.org/packages/IT.Legends.Zitadel.AccessTokenManagement)

Automatic service account token management. Tokens are acquired, cached, and refreshed automatically. Supports PAT, client credentials, and JWT Profile auth (including external signing via Azure Key Vault or similar). Can be used to call APIs protected by ZITADEL or the ZITADEL API itself..

```shell
dotnet add package IT.Legends.Zitadel.AccessTokenManagement
```

### [IT.Legends.Zitadel.Abstractions](./src/Zitadel.Abstractions/README.md)
[![NuGet](https://img.shields.io/nuget/v/IT.Legends.Zitadel.Abstractions.svg)](https://www.nuget.org/packages/IT.Legends.Zitadel.Abstractions)

Shared types and primitives — claim constants, scope definitions, credential models, and auth scheme defaults. Zero external dependencies. You generally don't need to reference this directly.

```shell
dotnet add package IT.Legends.Zitadel.Abstractions
```

### [IT.Legends.Zitadel.Clients](./src/Zitadel.Clients/README.md)
[![NuGet](https://img.shields.io/nuget/v/IT.Legends.Zitadel.Clients.svg)](https://www.nuget.org/packages/IT.Legends.Zitadel.Clients)

gRPC clients automatically generated from the ZITADEL proto files — one client per ZITADEL service. Use alongside `Zitadel.AccessTokenManagement` to call ZITADEL APIs from a backend service.


```shell
dotnet add package IT.Legends.Zitadel.Clients
```

---

## Examples

The [`examples`](./examples) folder contains working samples:

| Example | What it shows |
|---|---|
| [IT.Legends.Zitadel.AspNet.AuthN](./examples/Zitadel.AspNet.AuthN) | Web app with OIDC sign-in via `AddZitadel()` |
| [IT.Legends.Zitadel.WebApi](./examples/Zitadel.WebApi) | API protected with OAuth2 token introspection |
| [IT.Legends.Zitadel.ApiAccess](./examples/Zitadel.ApiAccess) | Backend service calling the ZITADEL gRPC API with a service account |

---

## Development

### Prerequisites

1. [.NET SDK](https://dotnet.microsoft.com/download) (10.x or later)
2. [Buf CLI](https://buf.build/docs/installation) — for protobuf code generation

### Getting started

```bash
# install buf first https://buf.build/docs/cli/quickstart/

ZITADEL_VERSION=v4.13.1

buf generate https://github.com/zitadel/zitadel.git#ref=${ZITADEL_VERSION:-main},depth=1 --include-imports --path ./proto/zitadel      # generate gRPC code (required before first build of Zitadel.Clients)
dotnet build
dotnet test --configuration Release
```

## Special Thanks

- buehler for his efforts on zitadel-net.
- Duende for their awesome foss identitymodel packages.


---

##### License

These libraries are licensed under the [Apache 2.0 License](LICENSE).
