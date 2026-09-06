using System.Text.Json.Serialization;

namespace RevitLookup.Abstractions.Settings;

/// <summary>
///     Represents the LookupEngine decomposition settings.
/// </summary>
[PublicAPI]
public sealed class DecompositionSettings
{
    /// <summary>
    ///     Gets or sets a value indicating whether non-public members are included in the decomposition result.
    /// </summary>
    [JsonPropertyName("IncludePrivate")]
    public bool IncludePrivate { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether field members are included in the decomposition result.
    /// </summary>
    [JsonPropertyName("IncludeFields")]
    public bool IncludeFields { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether static members are included in the decomposition result.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("IncludeStatic")]
    public bool IncludeStatic { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether event members are included in the decomposition result.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("IncludeEvents")]
    public bool IncludeEvents { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether synthetic extension members registered by descriptors are included in the decomposition result.
    /// </summary>
    /// <value>The default is <see langword="true" />.</value>
    [JsonPropertyName("IncludeExtensions")]
    public bool IncludeExtensions { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether unsupported and disabled members are included in the decomposition result.
    /// </summary>
    [JsonPropertyName("IncludeUnsupported")]
    public bool IncludeUnsupported { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether <see cref="object" /> itself is included at the top of the type hierarchy.
    /// </summary>
    [JsonPropertyName("IncludeRoot")]
    public bool IncludeRoot { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the member evaluation time column is shown.
    /// </summary>
    [JsonPropertyName("ShowTimeColumn")]
    public bool ShowTimeColumn { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the member allocated memory column is shown.
    /// </summary>
    [JsonPropertyName("ShowMemoryColumn")]
    public bool ShowMemoryColumn { get; set; }
}
