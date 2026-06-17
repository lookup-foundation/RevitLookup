using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RevitLookup.Abstractions.ObservableModels.Entries;
using RevitLookup.UI.Framework.Views.AboutProgram;
using RevitLookup.UI.Framework.Views.EditDialogs;
using RevitLookup.UI.Framework.Views.Tools;
using RevitLookup.UI.Framework.Views.Visualization;

namespace RevitLookup.UI.Playground.ViewModels.Pages;

[UsedImplicitly]
public sealed partial class DialogsViewModel(IServiceProvider serviceProvider) : ObservableObject
{
    [RelayCommand]
    private async Task ShowOpenSourceDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<OpenSourceDialog>();
        await dialog.ShowAsync();
    }

    [RelayCommand]
    private async Task ShowSearchElementsDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<SearchElementsDialog>();
        await dialog.ShowAsync();
    }

    [RelayCommand]
    private async Task ShowModulesDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<ModulesDialog>();
        await dialog.ShowAsync();
    }

    [RelayCommand]
    private async Task ShowParametersDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<UnitsDialog>();
        await dialog.ShowParametersDialogAsync();
    }

    [RelayCommand]
    private async Task ShowCategoriesDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<UnitsDialog>();
        await dialog.ShowCategoriesDialogAsync();
    }

    [RelayCommand]
    private async Task ShowForgeSchemaDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<UnitsDialog>();
        await dialog.ShowForgeSchemaDialogAsync();
    }

    [RelayCommand]
    private async Task ShowEditValueDialogAsync()
    {
        var faker = new Faker();
        var dialog = serviceProvider.GetRequiredService<EditValueDialog>();
        await dialog.ShowAsync(faker.Lorem.Word(), faker.Lorem.Sentence(5), "Update the parameter");
    }

    [RelayCommand]
    private async Task ShowCreateIniEntryDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<EditSettingsEntryDialog>();
        await dialog.ShowCreateDialogAsync(null);
    }

    [RelayCommand]
    private async Task ShowUpdateIniEntryDialogAsync()
    {
        var entry = new Faker<ObservableIniEntry>()
            .RuleFor(entry => entry.Category, faker => faker.Commerce.Product())
            .RuleFor(entry => entry.Property, faker => faker.Lorem.Word())
            .RuleFor(entry => entry.Value, faker => faker.Lorem.Word());

        var dialog = serviceProvider.GetRequiredService<EditSettingsEntryDialog>();
        await dialog.ShowUpdateDialogAsync(entry);
    }

    [RelayCommand]
    private async Task ShowBoundingBoxVisualizationDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<BoundingBoxVisualizationDialog>();
        await dialog.ShowDialogAsync("boundingBox");
    }

    [RelayCommand]
    private async Task ShowFaceVisualizationDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<FaceVisualizationDialog>();
        await dialog.ShowDialogAsync("face");
    }

    [RelayCommand]
    private async Task ShowMeshVisualizationDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<MeshVisualizationDialog>();
        await dialog.ShowDialogAsync("mesh");
    }

    [RelayCommand]
    private async Task ShowPolylineVisualizationDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<PolylineVisualizationDialog>();
        await dialog.ShowDialogAsync("polyline");
    }

    [RelayCommand]
    private async Task ShowCurveLoopVisualizationDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<CurveLoopVisualizationDialog>();
        await dialog.ShowDialogAsync("curveLoop");
    }

    [RelayCommand]
    private async Task ShowSolidVisualizationDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<SolidVisualizationDialog>();
        await dialog.ShowDialogAsync("solid");
    }

    [RelayCommand]
    private async Task ShowXyzVisualizationDialogAsync()
    {
        var dialog = serviceProvider.GetRequiredService<XyzVisualizationDialog>();
        await dialog.ShowDialogAsync("point");
    }
}