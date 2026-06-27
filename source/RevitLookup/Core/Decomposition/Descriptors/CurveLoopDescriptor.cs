using System.Globalization;
using System.Windows.Input;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Visualization;
using ContextMenu = System.Windows.Controls.ContextMenu;
#if REVIT2023_OR_GREATER
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
#endif

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed partial class CurveLoopDescriptor : Descriptor, IDescriptorConfigurator, IDescriptorConfigurator<Document>, IContextMenuConnector
{
    private readonly CurveLoop _curveLoop;

    public CurveLoopDescriptor(CurveLoop curveLoop)
    {
        _curveLoop = curveLoop;
        Name = $"{curveLoop.GetExactLength().ToString(CultureInfo.InvariantCulture)} ft";
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(CurveLoop.IsOpen)).Resolve(() => _curveLoop.IsOpen());
        configuration.Member(nameof(CurveLoop.GetPlane)).Resolve(() => _curveLoop.GetPlane());
        configuration.Member(nameof(CurveLoop.NumberOfCurves)).Resolve(() => _curveLoop.NumberOfCurves());

#if REVIT2022_OR_GREATER
        configuration.Extension("IsValidHorizontalBoundary").Register(() => BoundaryValidation.IsValidHorizontalBoundary([_curveLoop]));
#endif
#if REVIT2023_OR_GREATER
        configuration.Extension("IsValidBoundaryOnSketchPlane").Map(nameof(BoundaryValidation.IsValidBoundaryOnSketchPlane)).NotSupported();
#endif
    }

    void IDescriptorConfigurator<Document>.Configure(IMemberConfigurator<Document> configuration)
    {
#if REVIT2023_OR_GREATER
        configuration.Extension(nameof(BoundaryValidation.IsValidBoundaryOnView)).Register(context => BoundaryValidation.IsValidBoundaryOnView(context, context.ActiveView.Id, [_curveLoop]));
#endif
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
#if REVIT2023_OR_GREATER
        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_curveLoop, curveLoop => SelectCurveLoopEvent.Raise(curveLoop))
            .SetShortcut(Key.F6);

        contextMenu.AddMenuItem("ShowMenuItem")
            .SetCommand(_curveLoop, curveLoop => ShowCurveLoopEvent.Raise(curveLoop))
            .SetShortcut(Key.F7);
#endif
        contextMenu.AddMenuItem("VisualizeMenuItem")
            .SetAvailability(_curveLoop.GetExactLength() > 1e-6)
            .SetCommand(_curveLoop, loop => VisualizeCurve(loop, serviceProvider))
            .SetShortcut(Key.F8);
    }

    private static async Task VisualizeCurve(CurveLoop curveLoop, IServiceProvider serviceProvider)
    {
        if (RevitContext.ActiveUiDocument is null) return;

        try
        {
            var dialog = serviceProvider.GetRequiredService<CurveLoopVisualizationDialog>();
            await dialog.ShowDialogAsync(curveLoop);
        }
        catch (Exception exception)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<CurveLoopDescriptor>>();
            var notificationService = serviceProvider.GetRequiredService<INotificationService>();

            LogVisualizeCurveLoopError(logger, exception);
            notificationService.ShowError("Visualization error", exception);
        }
    }
#if REVIT2023_OR_GREATER

    [ExternalEvent(AllowDirectInvocation = true)]
    private static void SelectCurveLoop(UIApplication application, CurveLoop curveLoop)
    {
        if (application.ActiveUIDocument is null) return;

        var references = curveLoop.Where(curve => curve.Reference is not null).Select(curve => curve.Reference).ToArray();
        if (references.Length == 0) return;

        application.ActiveUIDocument.Selection.SetReferences(references);
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private static void ShowCurveLoop(UIApplication application, CurveLoop curveLoop)
    {
        var uiDocument = application.ActiveUIDocument;
        if (uiDocument is null) return;

        var curves = curveLoop.Where(curve => curve.Reference is not null).ToArray();
        if (curves.Length == 0) return;

        var elements = curves.Select(curve => curve.Reference.ElementId.ToElement(uiDocument.Document))
            .Where(element => element is not null)
            .Select(element => element!.Id)
            .ToArray();

        if (elements.Length > 0)
        {
            uiDocument.ShowElements(elements);
        }

        var references = curves.Select(curve => curve.Reference).ToArray();
        if (references.Length > 0)
        {
            uiDocument.Selection.SetReferences(references);
        }
    }
#endif

    [LoggerMessage(LogLevel.Error, "Visualize curve loop error")]
    private static partial void LogVisualizeCurveLoopError(ILogger<CurveLoopDescriptor> logger, Exception exception);
}