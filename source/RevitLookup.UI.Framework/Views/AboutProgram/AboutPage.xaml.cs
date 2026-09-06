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

using RevitLookup.Abstractions.Presentation;
using RevitLookup.Abstractions.ViewModels.AboutProgram;
using Wpf.Ui.Abstractions.Controls;

namespace RevitLookup.UI.Framework.Views.AboutProgram;

/// <summary>
///     Represents a page that shows information about the application, including its version and license.
/// </summary>
public sealed partial class AboutPage : INavigableView<IAboutViewModel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AboutPage" /> class.
    /// </summary>
    /// <param name="viewModel">The view model that provides the data for the About view.</param>
    /// <param name="themeWatcherService">The service that applies and tracks the current theme for this page.</param>
    public AboutPage(IAboutViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        themeWatcherService.Watch(this);

        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    /// <inheritdoc />
    public IAboutViewModel ViewModel { get; }
}
