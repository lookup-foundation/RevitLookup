namespace Build.Options;

/// <summary>
///     Represents the Azure Key Vault credentials used to sign build artifacts.
/// </summary>
[PublicAPI]
public sealed class SigningOptions
{
    /// <summary>
    ///     Gets the Azure Key Vault URI.
    /// </summary>
    public string? VaultUri { get; init; }

    /// <summary>
    ///     Gets the Azure Key Vault tenant ID.
    /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    ///     Gets the Azure Key Vault client ID.
    /// </summary>
    public string? ClientId { get; init; }

    /// <summary>
    ///     Gets the Azure Key Vault client secret.
    /// </summary>
    public string? ClientSecret { get; init; }

    /// <summary>
    ///     Gets the Azure Key Vault certificate name.
    /// </summary>
    public string? CertificateName { get; init; }
}
