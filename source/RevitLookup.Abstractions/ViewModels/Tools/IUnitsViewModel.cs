using RevitLookup.Abstractions.Tools;

namespace RevitLookup.Abstractions.ViewModels.Tools;

/// <summary>
///     Defines a contract that represents the data for the Units view.
/// </summary>
public interface IUnitsViewModel
{
    /// <summary>
    ///     Gets or sets the list of all units.
    /// </summary>
    List<UnitInfo> Units { get; set; }

    /// <summary>
    ///     Gets or sets the list of filtered units.
    /// </summary>
    List<UnitInfo> FilteredUnits { get; set; }

    /// <summary>
    ///     Gets or sets the search query used to filter units.
    /// </summary>
    string SearchText { get; set; }

    /// <summary>
    ///     Populates <see cref="Units" /> with the built-in parameters.
    /// </summary>
    void InitializeParameters();

    /// <summary>
    ///     Populates <see cref="Units" /> with the built-in categories.
    /// </summary>
    void InitializeCategories();

    /// <summary>
    ///     Populates <see cref="Units" /> with the Forge schema.
    /// </summary>
    void InitializeForgeSchema();

    /// <summary>
    ///     Decomposes the specified unit information and visualizes it.
    /// </summary>
    /// <param name="unitInfo">The unit information to decompose.</param>
    /// <returns>A task that represents the asynchronous decompose operation.</returns>
    Task DecomposeAsync(UnitInfo unitInfo);
}
