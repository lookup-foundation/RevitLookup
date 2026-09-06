using CommunityToolkit.Mvvm.ComponentModel;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Abstractions.Decomposition;

/// <summary>
///     Represents the observable model for the LookupEngine decomposed value.
/// </summary>
public sealed class ObservableDecomposedValue : ObservableObject
{
    /// <summary>
    ///     Gets or sets the underlying CLR object this model represents.
    /// </summary>
    public required object? RawValue { get; init; }

    /// <summary>
    ///     Gets or sets the value's display name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the simple name of the value's type.
    /// </summary>
    public required string TypeName { get; set; }

    /// <summary>
    ///     Gets or sets the fully qualified name of the value's type.
    /// </summary>
    public required string TypeFullName { get; set; }

    /// <summary>
    ///     Gets or sets the value's description, or <see langword="null" /> when none is available.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the engine descriptor used to decompose this value, or <see langword="null" /> when none applies.
    /// </summary>
    public Descriptor? Descriptor { get; set; }
}
