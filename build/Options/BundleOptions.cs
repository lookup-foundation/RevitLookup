namespace Build.Options;

/// <summary>
///     Information about the Autodesk application package.
/// </summary>
/// <seealso href="https://www.autodesk.com/autodesk-university/class/AppBundle-Cross-Distribution-Autodesk-Products-App-Store-and-Forge-2020">AppBundle: Cross-Distribution Autodesk Products, App Store, and Forge</seealso>
[Serializable]
public sealed record BundleOptions
{
    /// <summary>
    ///     Gets the vendor name.
    /// </summary>
    public string? VendorName { get; init; }

    /// <summary>
    ///     Gets the vendor website URL.
    /// </summary>
    public string? VendorUrl { get; init; }

    /// <summary>
    ///     Gets the vendor email address.
    /// </summary>
    public string? VendorEmail { get; init; }
}
