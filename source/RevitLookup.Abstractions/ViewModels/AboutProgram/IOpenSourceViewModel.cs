using RevitLookup.Abstractions.AboutProgram;

namespace RevitLookup.Abstractions.ViewModels.AboutProgram;

/// <summary>
///     Defines a contract that represents the data for the OpenSource view.
/// </summary>
public interface IOpenSourceViewModel
{
    /// <summary>
    ///     Gets the list of open-source software used in the application.
    /// </summary>
    List<OpenSourceSoftware> Software { get; }
}
