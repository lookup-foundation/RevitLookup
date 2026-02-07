using System.Globalization;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Configuration;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Services.Application;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.Visualization;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class CurveLoopDescriptor : Descriptor, IDescriptorResolver, IContextMenuConnector
{
    private readonly CurveLoop _curveLoop;

    public CurveLoopDescriptor(CurveLoop curveLoop)
    {
        _curveLoop = curveLoop;
        Name = $"{curveLoop.GetExactLength().ToString(CultureInfo.InvariantCulture)} ft";
    }

    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(CurveLoop.IsOpen) => ResolveIsOpen,
            nameof(CurveLoop.GetPlane) => ResolveGetPlane,
            nameof(CurveLoop.NumberOfCurves) => ResolveNumberOfCurves,
            _ => null
        };

        IVariant ResolveNumberOfCurves()
        {
            var variants = Variants.Values<int>(1);

            variants.Add(_curveLoop.NumberOfCurves(), "number of curves in the curve loop");

            return variants.Consume();
        }

        IVariant ResolveIsOpen()
        {
            return Variants.Values<bool>(1)
                .Add(_curveLoop.IsOpen(), "whether the curve loop is open or closed")
                .Consume();
        }

        IVariant ResolveGetPlane()
        {
            return Variants.Values<Plane>(1)
                .Add(_curveLoop.GetPlane(), "Plane")
                .Consume();
        }
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
#if REVIT2023_OR_GREATER
        contextMenu.AddMenuItem("SelectMenuItem")
            .SetCommand(_curveLoop, SelectCurve)
            .SetShortcut(Key.F6);

        contextMenu.AddMenuItem("ShowMenuItem")
            .SetCommand(_curveLoop, ShowCurve)
            .SetShortcut(Key.F7);
#endif
        contextMenu.AddMenuItem("VisualizeMenuItem")
            .SetAvailability(_curveLoop.GetExactLength() > 1e-6)
            .SetCommand(_curveLoop, VisualizeCurve)
            .SetShortcut(Key.F8);

        async Task VisualizeCurve(CurveLoop curveLoop)
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

                logger.LogError(exception, "Visualize curve loop error");
                notificationService.ShowError("Visualization error", exception);
            }
        }

#if REVIT2023_OR_GREATER
        void SelectCurve(CurveLoop curveLoop)
        {
            if (RevitContext.ActiveUiDocument is null) return;
            if (curveLoop.Any(curve => curve.Reference is null)) return;

            foreach (var curve in curveLoop)
            {
                EventHandlers.ActionEventHandler.Raise(_ => RevitContext.ActiveUiDocument.Selection.SetReferences([curve.Reference]));
            }
        }

        void ShowCurve(CurveLoop curveLoop)
        {
            if (RevitContext.ActiveUiDocument is null) return;
            if (curveLoop.Any(curve => curve.Reference is null)) return;

            EventHandlers.ActionEventHandler.Raise(application =>
            {
                var uiDocument = application.ActiveUIDocument;
                if (uiDocument is null) return;

                foreach (var curve in curveLoop)
                {
                    var element = curve.Reference.ElementId.ToElement(uiDocument.Document);
                    if (element is not null) uiDocument.ShowElements(element);

                    uiDocument.Selection.SetReferences([curve.Reference]);
                }
            });
        }
#endif
    }
}