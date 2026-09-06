namespace RevitLookup.Abstractions.Decomposition;

/// <summary>
///     Determines which known Revit object is resolved and decomposed.
/// </summary>
public enum KnownDecompositionObject
{
    /// <summary>
    ///     Resolves the active view.
    /// </summary>
    View,

    /// <summary>
    ///     Resolves the active document.
    /// </summary>
    Document,

    /// <summary>
    ///     Resolves the Revit application.
    /// </summary>
    Application,

    /// <summary>
    ///     Resolves the active UI application.
    /// </summary>
    UiApplication,

    /// <summary>
    ///     Resolves the UI controlled application.
    /// </summary>
    UiControlledApplication,

    /// <summary>
    ///     Resolves every element type and instance in the active document.
    /// </summary>
    Database,

    /// <summary>
    ///     Resolves the elements dependent on the current selection.
    /// </summary>
    DependentElements,

    /// <summary>
    ///     Resolves the selected elements, or all elements in the active view when nothing is selected.
    /// </summary>
    Selection,

    /// <summary>
    ///     Resolves a face picked interactively from an element.
    /// </summary>
    Face,

    /// <summary>
    ///     Resolves an edge picked interactively from an element.
    /// </summary>
    Edge,

    /// <summary>
    ///     Resolves a point picked interactively on an element.
    /// </summary>
    Point,

    /// <summary>
    ///     Resolves a sub-element picked interactively from an element.
    /// </summary>
    SubElement,

    /// <summary>
    ///     Resolves an element picked interactively from a linked document.
    /// </summary>
    LinkedElement,

    /// <summary>
    ///     Resolves the Autodesk Windows component manager.
    /// </summary>
    ComponentManager,

    /// <summary>
    ///     Resolves the Revit performance adviser.
    /// </summary>
    PerformanceAdviser,

    /// <summary>
    ///     Resolves the registered document updaters.
    /// </summary>
    UpdaterRegistry,

    /// <summary>
    ///     Resolves the registered external services.
    /// </summary>
    Services,

    /// <summary>
    ///     Resolves the registered extensible storage schemas.
    /// </summary>
    Schemas
}
