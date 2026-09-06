using Autodesk.Revit.UI;

namespace RevitLookup.Commands.Controllers;

/// <summary>
///     Represents the availability controller that reports a Revit ribbon command as always available.
/// </summary>
public sealed class CommandAlwaysAvailableController : IExternalCommandAvailability
{
    /// <inheritdoc />
    public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
    {
        return true;
    }
}
