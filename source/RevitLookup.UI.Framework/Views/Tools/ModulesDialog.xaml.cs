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

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.Tools;
using RevitLookup.Abstractions.ViewModels.Tools;
using RevitLookup.UI.Framework.Menus;
using RevitLookup.UI.Framework.Processes;
using Wpf.Ui;

namespace RevitLookup.UI.Framework.Views.Tools;

/// <summary>
///     Represents a dialog that lists the assemblies loaded into the current process.
/// </summary>
public sealed partial class ModulesDialog
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ModulesDialog" /> class.
    /// </summary>
    /// <param name="dialogService">The service that supplies the dialog host this dialog is shown on.</param>
    /// <param name="viewModel">The view model that provides the data for the Modules view.</param>
    /// <param name="themeWatcherService">The service that applies and tracks the current theme for this dialog.</param>
    public ModulesDialog(
        IContentDialogService dialogService,
        IModulesViewModel viewModel, IThemeWatcherService themeWatcherService)
        : base(dialogService.GetDialogHostEx())
    {
        DataContext = viewModel;
        InitializeComponent();

        themeWatcherService.Watch(this);

#if NETFRAMEWORK
        var header = (TextBlock)ContainerColumn.Header;
        header.Text = "Domain";
#endif
    }

    private void OnMouseEnter(object sender, RoutedEventArgs args)
    {
        var element = (FrameworkElement)sender;
        var moduleInfo = (ModuleInfo)element.DataContext;
        CreateRowContextMenu(moduleInfo, element);
    }

    private void CreateRowContextMenu(ModuleInfo module, FrameworkElement row)
    {
        var contextMenu = new ContextMenu
        {
            Resources = UiApplication.Current.Resources,
            PlacementTarget = row
        };

        var copyMenu = contextMenu.AddMenuItem("CopyMenuItem")
            .SetHeader("Copy");

        copyMenu.AddMenuItem()
            .SetHeader("Module name")
            .SetCommand(module, moduleInfo => Clipboard.SetDataObject(moduleInfo.Name))
            .SetShortcut(ModifierKeys.Control, Key.C);

        copyMenu.AddMenuItem()
            .SetHeader("Module location")
            .SetCommand(module, moduleInfo => Clipboard.SetDataObject(moduleInfo.Path))
            .SetShortcut(ModifierKeys.Control | ModifierKeys.Shift, Key.C);

        copyMenu.AddMenuItem()
            .SetHeader("Module version")
            .SetCommand(module, moduleInfo => Clipboard.SetDataObject(moduleInfo.Version));

#if NET
        copyMenu.AddMenuItem()
            .SetHeader("Assembly load сontext")
            .SetCommand(module, moduleInfo => Clipboard.SetDataObject(moduleInfo.Container));
#endif

        var navigateMenu = contextMenu.AddMenuItem()
            .SetHeader("Navigate");

        navigateMenu.AddMenuItem()
            .SetHeader("Module directory")
            .SetAvailability(File.Exists(module.Path))
            .SetCommand(module, moduleInfo => ProcessTasks.StartShell(Path.GetDirectoryName(moduleInfo.Path)!));

        navigateMenu.AddMenuItem()
            .SetHeader("Module location")
            .SetAvailability(File.Exists(module.Path))
            .SetCommand(module, moduleInfo => ProcessTasks.StartShell("explorer.exe", $"/select,{moduleInfo.Path}"));

        row.ContextMenu = contextMenu;
    }
}
