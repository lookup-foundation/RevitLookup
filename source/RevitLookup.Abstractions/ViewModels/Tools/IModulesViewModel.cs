using RevitLookup.Abstractions.Tools;

namespace RevitLookup.Abstractions.ViewModels.Tools;

/// <summary>
///     Defines a contract that represents the data for the Modules view.
/// </summary>
public interface IModulesViewModel
{
    /// <summary>
    ///     Gets or sets the search query used to filter modules.
    /// </summary>
    string SearchText { get; set; }

    /// <summary>
    ///     Gets or sets the list of filtered modules.
    /// </summary>
    List<ModuleInfo> FilteredModules { get; set; }

    /// <summary>
    ///     Gets or sets the list of all assembly modules.
    /// </summary>
    List<ModuleInfo> Modules { get; set; }
}
