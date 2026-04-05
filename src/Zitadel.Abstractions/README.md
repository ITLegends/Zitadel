# IT.Legends.Zitadel.Abstractions

Shared types and primitives for [ZITADEL](https://zitadel.com) .NET integrations. Zero external dependencies, used by both `IT.Legends.Zitadel.AspNetCore` and `IT.Legends..AccessTokenManagement`.

[![NuGet](https://img.shields.io/nuget/v/IT.Legends.Zitadel.Abstractions.svg)](https://www.nuget.org/packages/IT.Legends.Zitadel.Abstractions)
[![NuGet Downloads](https://img.shields.io/nuget/dt/IT.Legends.Zitadel.Abstractions.svg)](https://www.nuget.org/packages/IT.Legends.Zitadel.Abstractions)

---
## What’s included

This package provides a focused set of primitives for working with ZITADEL in .NET applications:

- **Claim types** for reading and constructing ZITADEL claim values
- **Scopes** for common OpenID Connect and ZITADEL-specific permissions
- **Headers** for API calls that need organization context
- **Defaults** for consistent authentication scheme names and callback paths
- **Credentials models** for application and service account configuration
- **JWT signing delegate** for scenarios where signing of JWT is handled externally

