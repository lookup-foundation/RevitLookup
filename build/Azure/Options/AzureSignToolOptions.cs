using ModularPipelines.Attributes;
using ModularPipelines.Options;

namespace Build.Azure.Options;

/// <summary>
///     Represents the command-line options for the AzureSignTool <c>sign</c> command.
/// </summary>
/// <remarks>
///     The command code-signs a set of files with a certificate stored in an Azure Key Vault instance.
/// </remarks>
[PublicAPI]
[Serializable]
[CliSubCommand("sign")]
public sealed record AzureSignToolOptions : CommandLineToolOptions
{
    /// <summary>
    ///     Gets or sets the URL of the Azure Key Vault instance that holds the signing certificate.
    /// </summary>
    [CliOption("--azure-key-vault-url")]
    public string? KeyVaultUrl { get; set; }

    /// <summary>
    ///     Gets or sets the client ID used to authenticate with the Azure Key Vault instance.
    /// </summary>
    [CliOption("--azure-key-vault-client-id")]
    public string? KeyVaultClientId { get; set; }

    /// <summary>
    ///     Gets or sets the client secret used to authenticate with the Azure Key Vault instance.
    /// </summary>
    [CliOption("--azure-key-vault-client-secret")]
    public string? KeyVaultClientSecret { get; set; }

    /// <summary>
    ///     Gets or sets the Azure Active Directory tenant ID used to authenticate with the Azure Key Vault instance.
    /// </summary>
    [CliOption("--azure-key-vault-tenant-id")]
    public string? KeyVaultTenantId { get; set; }

    /// <summary>
    ///     Gets or sets the name of the certificate in the Azure Key Vault instance used to sign the files.
    /// </summary>
    [CliOption("--azure-key-vault-certificate")]
    public string? KeyVaultCertificateName { get; set; }

    /// <summary>
    ///     Gets or sets the access token used to authenticate with the Azure Key Vault instance in place of a client ID and secret.
    /// </summary>
    [CliOption("--azure-key-vault-accesstoken")]
    public string? KeyVaultAccessToken { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the Azure Key Vault instance is authenticated using the current Azure-managed identity.
    /// </summary>
    [CliFlag("--azure-key-vault-managed-identity")]
    public bool? KeyVaultManagedIdentity { get; set; }

    /// <summary>
    ///     Gets or sets the description embedded in the Authenticode signature.
    /// </summary>
    [CliOption("--description")]
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the URL embedded in the Authenticode signature.
    /// </summary>
    [CliOption("--description-url")]
    public string? DescriptionUrl { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the RFC 3161 timestamp server used to timestamp the signature.
    /// </summary>
    [CliOption("--timestamp-rfc3161")]
    public string? TimestampRfc3161Url { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the Authenticode timestamp server used to timestamp the signature.
    /// </summary>
    [CliOption("--timestamp-authenticode")]
    public string? TimestampAuthenticodeUrl { get; set; }

    /// <summary>
    ///     Gets or sets the digest algorithm used to sign the timestamp.
    /// </summary>
    [CliOption("--timestamp-digest")]
    public string? TimestampDigest { get; set; }

    /// <summary>
    ///     Gets or sets the digest algorithm used to sign the files.
    /// </summary>
    [CliOption("--file-digest")]
    public string? FileDigest { get; set; }

    /// <summary>
    ///     Gets or sets the paths to additional certificates included in the signature chain.
    /// </summary>
    [CliOption("--additional-certificates")]
    public IEnumerable<string>? AdditionalCertificates { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether verbose output is printed while signing.
    /// </summary>
    [CliFlag("--verbose")]
    public bool? Verbose { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether all output except errors is suppressed while signing.
    /// </summary>
    [CliFlag("--quiet")]
    public bool? Quiet { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether signing continues with the remaining files after one file fails to sign.
    /// </summary>
    [CliFlag("--continue-on-error")]
    public bool? ContinueOnError { get; set; }

    /// <summary>
    ///     Gets or sets the path to a file listing the paths of the files to sign, one per line.
    /// </summary>
    [CliOption("--input-file-list")]
    public string? InputFileList { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether a file that is already signed is skipped.
    /// </summary>
    [CliFlag("--skip-signed")]
    public bool? SkipSigned { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the signature is appended to a file that is already signed instead of replacing it.
    /// </summary>
    [CliFlag("--append-signature")]
    public bool? AppendSignature { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether page hashing is enabled for the signature.
    /// </summary>
    [CliFlag("--page-hashing")]
    public bool? PageHashing { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether page hashing is disabled for the signature.
    /// </summary>
    [CliFlag("--no-page-hashing")]
    public bool? NoPageHashing { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of files signed concurrently.
    /// </summary>
    [CliOption("--max-degree-of-parallelism")]
    public int? MaxDegreeOfParallelism { get; set; }
}
