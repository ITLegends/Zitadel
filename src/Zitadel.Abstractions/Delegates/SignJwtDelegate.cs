namespace Zitadel.Abstractions.Delegates;

/// <summary>
/// A delegate that can be used to offload the signing of JSON Web Tokens to an external vault.
/// </summary>
public delegate Task<byte[]> SignJwtAsync(
    IServiceProvider serviceProvider,
    byte[] data,
    CancellationToken ct = default);
