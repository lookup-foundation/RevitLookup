// Copyright (c) Lookup Foundation and Contributors
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// THIS PROGRAM IS PROVIDED "AS IS" AND WITH ALL FAULTS.
// NO IMPLIED WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE IS PROVIDED.
// THERE IS NO GUARANTEE THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LookupEngine.Abstractions.Enums;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.ObservableModels.Decomposition;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Utils;
using Wpf.Ui;

namespace RevitLookup.UI.Framework.Views.Decomposition;

public partial class SummaryViewBase
{
    /// <summary>
    ///     Tree view context menu
    /// </summary>
    private void CreateTreeContextMenu(ObservableDecomposedObject decomposedObject, FrameworkElement row)
    {
        var contextMenu = new ContextMenu
        {
            PlacementTarget = row,
            Resources = UiApplication.Current.Resources
        };

        row.ContextMenu = contextMenu;

        contextMenu.AddMenuItem("CopyMenuItem")
            .SetCommand(decomposedObject, parameter => Clipboard.SetDataObject(parameter.Name))
            .SetShortcut(ModifierKeys.Control, Key.C);
        contextMenu.AddMenuItem("HelpMenuItem")
            .SetCommand(decomposedObject, parameter => HelpUtils.ShowHelp(parameter.TypeFullName))
            .SetShortcut(Key.F1);

        if (decomposedObject.Descriptor is not IContextMenuConnector connector) return;

        try
        {
            connector.RegisterMenu(contextMenu, _serviceProvider);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to register the context menu");
            _notificationService.ShowError("Failed to register the context menu", exception);
        }
    }

    /// <summary>
    ///     Data grid row context menu
    /// </summary>
    private void CreateGridRowContextMenu(ObservableDecomposedMember member, FrameworkElement row)
    {
        var contextMenu = new ContextMenu
        {
            PlacementTarget = row,
            Resources = UiApplication.Current.Resources,
        };

        row.ContextMenu = contextMenu;

        if (member.EvaluationPolicy == MemberEvaluationPolicy.Deferred)
        {
            contextMenu.AddMenuItem("EvaluateMenuItem")
                .SetCommand(member, async parameter => await ViewModel.ForceEvaluateMemberCommand.ExecuteAsync(parameter));
            
            contextMenu.AddSeparator();
        }

        contextMenu.AddMenuItem("CopyMenuItem")
            .SetCommand(member, parameter => Clipboard.SetDataObject($"{parameter.Name}: {parameter.Value.Name}"))
            .SetShortcut(ModifierKeys.Control, Key.C)
            .SetAvailability(member.Value.Name != string.Empty);

        contextMenu.AddMenuItem("CopyMenuItem")
            .SetHeader("Copy value")
            .SetCommand(member, parameter => Clipboard.SetDataObject(parameter.Value.Name))
            .SetShortcut(ModifierKeys.Control | ModifierKeys.Shift, Key.C)
            .SetAvailability(member.Value.Name != string.Empty);

        contextMenu.AddMenuItem("HelpMenuItem")
            .SetCommand(member, parameter => HelpUtils.ShowHelp(parameter.DeclaringTypeFullName, parameter.Name))
            .SetShortcut(Key.F1);

        if (member.Value.Descriptor is not IContextMenuConnector connector) return;

        try
        {
            connector.RegisterMenu(contextMenu, _serviceProvider);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to register the context menu");
            _notificationService.ShowError("Failed to register the context menu", exception);
        }
    }

    /// <summary>
    ///     Data grid context menu
    /// </summary>
    private void CreateGridContextMenu(DataGrid dataGrid)
    {
        var contextMenu = new ContextMenu
        {
            PlacementTarget = dataGrid,
            Resources = UiApplication.Current.Resources
        };

        dataGrid.ContextMenu = contextMenu;

        contextMenu.AddMenuItem("RefreshMenuItem")
            .SetCommand(ViewModel, async parameter => await parameter.RefreshMembersAsync())
            .SetGestureText(Key.F5);

        contextMenu.AddSeparator();
        contextMenu.AddLabel("Columns");

        contextMenu.AddMenuItem()
            .SetHeader("Time")
            .SetStaysOpenOnClick(true)
            .SetChecked(dataGrid.Columns[2].Visibility == Visibility.Visible)
            .SetCommand(dataGrid.Columns[2], parameter =>
            {
                _settingsService.DecompositionSettings.ShowTimeColumn = parameter.Visibility != Visibility.Visible;
                parameter.Visibility = _settingsService.DecompositionSettings.ShowTimeColumn ? Visibility.Visible : Visibility.Collapsed;
            });

        contextMenu.AddMenuItem()
            .SetHeader("Memory")
            .SetStaysOpenOnClick(true)
            .SetChecked(dataGrid.Columns[3].Visibility == Visibility.Visible)
            .SetCommand(dataGrid.Columns[3], parameter =>
            {
                _settingsService.DecompositionSettings.ShowMemoryColumn = parameter.Visibility != Visibility.Visible;
                parameter.Visibility = _settingsService.DecompositionSettings.ShowMemoryColumn ? Visibility.Visible : Visibility.Collapsed;
            });

        contextMenu.AddSeparator();
        contextMenu.AddLabel("Show");

        contextMenu.AddMenuItem()
            .SetHeader("Events")
            .SetStaysOpenOnClick(true)
            .SetChecked(_settingsService.DecompositionSettings.IncludeEvents)
            .SetCommand(_settingsService.DecompositionSettings, async parameter =>
            {
                parameter.IncludeEvents = !parameter.IncludeEvents;
                await ViewModel.RefreshMembersAsync();
            });
        contextMenu.AddMenuItem()
            .SetHeader("Extensions")
            .SetStaysOpenOnClick(true)
            .SetChecked(_settingsService.DecompositionSettings.IncludeExtensions)
            .SetCommand(_settingsService.DecompositionSettings, async parameter =>
            {
                parameter.IncludeExtensions = !parameter.IncludeExtensions;
                await ViewModel.RefreshMembersAsync();
            });
        contextMenu.AddMenuItem()
            .SetHeader("Fields")
            .SetStaysOpenOnClick(true)
            .SetChecked(_settingsService.DecompositionSettings.IncludeFields)
            .SetCommand(_settingsService.DecompositionSettings, async parameter =>
            {
                parameter.IncludeFields = !parameter.IncludeFields;
                await ViewModel.RefreshMembersAsync();
            });
        contextMenu.AddMenuItem()
            .SetHeader("Non-public")
            .SetStaysOpenOnClick(true)
            .SetChecked(_settingsService.DecompositionSettings.IncludePrivate)
            .SetCommand(_settingsService.DecompositionSettings, async parameter =>
            {
                parameter.IncludePrivate = !parameter.IncludePrivate;
                await ViewModel.RefreshMembersAsync();
            });
        contextMenu.AddMenuItem()
            .SetHeader("Root")
            .SetStaysOpenOnClick(true)
            .SetChecked(_settingsService.DecompositionSettings.IncludeRoot)
            .SetCommand(_settingsService.DecompositionSettings, async parameter =>
            {
                parameter.IncludeRoot = !parameter.IncludeRoot;
                await ViewModel.RefreshMembersAsync();
            });
        contextMenu.AddMenuItem()
            .SetHeader("Static")
            .SetStaysOpenOnClick(true)
            .SetChecked(_settingsService.DecompositionSettings.IncludeStatic)
            .SetCommand(_settingsService.DecompositionSettings, async parameter =>
            {
                parameter.IncludeStatic = !parameter.IncludeStatic;
                await ViewModel.RefreshMembersAsync();
            });
        contextMenu.AddMenuItem()
            .SetHeader("Unsupported")
            .SetStaysOpenOnClick(true)
            .SetChecked(_settingsService.DecompositionSettings.IncludeUnsupported)
            .SetCommand(_settingsService.DecompositionSettings, async parameter =>
            {
                parameter.IncludeUnsupported = !parameter.IncludeUnsupported;
                await ViewModel.RefreshMembersAsync();
            });
    }
}