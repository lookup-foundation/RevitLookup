using System.Windows;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.Common.Extensions;
using RevitLookup.UI.Framework.Views.AboutProgram;
using RevitLookup.UI.Framework.Views.Dashboard;
using RevitLookup.UI.Framework.Views.Decomposition;
using RevitLookup.UI.Framework.Views.Settings;
using RevitLookup.UI.Framework.Views.Tools;
using RevitLookup.UI.Playground.Controls;

namespace RevitLookup.UI.Playground.ViewModels.Pages;

[UsedImplicitly]
public sealed partial class PagesViewModel(IServiceProvider serviceProvider) : ObservableObject
{
    [RelayCommand]
    private void ShowDashboardPage()
    {
        var viewer = serviceProvider.CreateScopedFrameworkElement<PageViewer>();
        viewer.SizeToContent = SizeToContent.Width;
        viewer.Height = 850;
        viewer.ShowPage<DashboardPage>();
    }

    [RelayCommand]
    private void ShowDecompositionSummaryPage()
    {
        var viewer = serviceProvider.CreateScopedFrameworkElement<PageViewer>();
        viewer.SizeToContent = SizeToContent.Manual;
        viewer.Height = 500;
        viewer.Width = 900;

        viewer.ShowPage<DecompositionSummaryPage>((page, provider) =>
        {
            var faker = new Faker();

            var strings = new List<string>();
            for (var i = 0; i < 1000; i++)
            {
                strings.Add(faker.Lorem.Sentence(300));
            }

            var decompositionService = provider.GetRequiredService<IVisualDecompositionService>();
            decompositionService.VisualizeDecompositionAsync(strings);
        });
    }

    [RelayCommand]
    private void ShowEventsSummaryPage()
    {
        var viewer = serviceProvider.CreateScopedFrameworkElement<PageViewer>();
        viewer.SizeToContent = SizeToContent.Manual;
        viewer.Height = 500;
        viewer.Width = 900;

        viewer.Closing += (sender, _) =>
        {
            var self = (PageViewer) sender!;
            var view = (EventsSummaryPage) self.Viewer.Content;
            var viewModel = (IEventsSummaryViewModel) view.ViewModel;
            viewModel.OnNavigatedFromAsync();
        };

        viewer.ShowPage<EventsSummaryPage>((page, provider) =>
        {
            var viewModel = (IEventsSummaryViewModel) page.ViewModel;
            viewModel.OnNavigatedToAsync();
        });
    }

    [RelayCommand]
    private void ShowSettingsPage()
    {
        var viewer = serviceProvider.CreateScopedFrameworkElement<PageViewer>();
        viewer.ShowPage<SettingsPage>();
    }

    [RelayCommand]
    private void ShowAboutPage()
    {
        var viewer = serviceProvider.CreateScopedFrameworkElement<PageViewer>();
        viewer.ShowPage<AboutPage>();
    }

    [RelayCommand]
    private void ShowRevitSettingsPage()
    {
        var viewer = serviceProvider.CreateScopedFrameworkElement<PageViewer>();
        viewer.SizeToContent = SizeToContent.Manual;
        viewer.Height = 850;
        viewer.Width = 550;
        viewer.ShowPage<RevitSettingsPage>();
    }
}