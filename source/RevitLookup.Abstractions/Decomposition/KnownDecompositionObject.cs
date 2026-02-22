namespace RevitLookup.Abstractions.Decomposition;

/// <summary>
///     Supported object types for decomposition.
/// </summary>
public enum KnownDecompositionObject
{
    View,
    Document,
    Application,
    UiApplication,
    UiControlledApplication,
    Database,
    DependentElements,
    Selection,
    Face,
    Edge,
    Point,
    SubElement,
    LinkedElement,
    ComponentManager,
    PerformanceAdviser,
    UpdaterRegistry,
    Services,
    Schemas
}