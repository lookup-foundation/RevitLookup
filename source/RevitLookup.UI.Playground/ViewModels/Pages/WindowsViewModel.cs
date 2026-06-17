using System.Collections;
using System.Numerics;
using System.Reflection;
using System.Windows.Media;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RevitLookup.Abstractions.Models.Settings;
using RevitLookup.Abstractions.Services.Application;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.UI.Framework.Views.Dashboard;
using RevitLookup.UI.Framework.Views.Decomposition;
using RevitLookup.UI.Playground.Mocks.Models;

namespace RevitLookup.UI.Playground.ViewModels.Pages;

[UsedImplicitly]
public sealed partial class WindowsViewModel : ObservableObject
{
    private const int SampleSize = 666;

    [RelayCommand]
    private void ShowRevitLookupWindow()
    {
        Host.GetService<IUiOrchestratorService>()
            .Show<DashboardPage>();
    }

    [RelayCommand]
    private void ShowEventsWindow()
    {
        Host.GetService<IUiOrchestratorService>()
            .Show<EventsSummaryPage>();
    }

    [RelayCommand]
    private void ShowDecomposeColorsWindow()
    {
        var faker = new Faker();

        var colors = new List<Color>(SampleSize);
        for (var i = 0; i < SampleSize; i++)
        {
            colors.Add(Color.FromArgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte()));
        }

        ApplyDefaultOptions();
        OpenSummary(colors);
    }

    [RelayCommand]
    private void ShowDecomposeTextWindow()
    {
        var faker = new Faker();

        var strings = new List<string>(SampleSize);
        for (var i = 0; i < SampleSize; i++)
        {
            strings.Add(faker.Lorem.Sentence());
        }

        ApplyDefaultOptions();
        OpenSummary(strings);
    }

    [RelayCommand]
    private void ShowDecomposeVectorsWindow()
    {
        var faker = new Faker();

        var vectors = new List<Vector3>(SampleSize);
        for (var i = 0; i < SampleSize; i++)
        {
            vectors.Add(new Vector3(faker.Random.Float(), faker.Random.Float(), faker.Random.Float()));
        }

        ApplyDefaultOptions();
        OpenSummary(vectors);
    }

    [RelayCommand]
    private void ShowDecomposeTypesWindow()
    {
        var assembly = Assembly.GetExecutingAssembly();

        ApplyDefaultOptions();
        OpenSummary(assembly.GetTypes());
    }

    [RelayCommand]
    private void ShowMembersWindow()
    {
        var settings = ResetEngineOptions();
        settings.IncludeRoot = true;
        settings.IncludeFields = true;
        settings.IncludeEvents = true;
        settings.IncludeUnsupported = true;
        settings.IncludePrivate = true;
        settings.IncludeStatic = true;
        settings.IncludeExtensions = true;

        OpenSummary(new MixedSample());
    }

    [RelayCommand]
    private void ShowDeferredMembersWindow()
    {
        ApplyDefaultOptions();
        OpenSummary(new DeferredSample());
    }

    [RelayCommand]
    private void ShowDisabledMembersWindow()
    {
        ApplyDefaultOptions();
        OpenSummary(new DisabledSample());
    }

    [RelayCommand]
    private void ShowUnsupportedMembersWindow()
    {
        var settings = ApplyDefaultOptions();
        settings.IncludeUnsupported = true;

        OpenSummary(new UnsupportedSample());
    }

    [RelayCommand]
    private void ShowPlaceholderValuesWindow()
    {
        ApplyDefaultOptions();
        OpenSummary(new PlaceholderSample());
    }

    [RelayCommand]
    private void ShowExceptionMembersWindow()
    {
        ApplyDefaultOptions();
        OpenSummary(new ExceptionSample());
    }

    [RelayCommand]
    private void ShowStaticOptionWindow()
    {
        var settings = ResetEngineOptions();
        settings.IncludeStatic = true;

        OpenSummary(new MixedSample());
    }

    [RelayCommand]
    private void ShowFieldsOptionWindow()
    {
        var settings = ResetEngineOptions();
        settings.IncludeFields = true;

        OpenSummary(new MixedSample());
    }

    [RelayCommand]
    private void ShowPrivateOptionWindow()
    {
        var settings = ResetEngineOptions();
        settings.IncludePrivate = true;

        OpenSummary(new MixedSample());
    }

    [RelayCommand]
    private void ShowEventsOptionWindow()
    {
        var settings = ResetEngineOptions();
        settings.IncludeEvents = true;

        OpenSummary(new MixedSample());
    }

    [RelayCommand]
    private void ShowRootOptionWindow()
    {
        var settings = ResetEngineOptions();
        settings.IncludeRoot = true;

        OpenSummary(new MixedSample());
    }

    [RelayCommand]
    private void ShowUnsupportedOptionWindow()
    {
        var settings = ResetEngineOptions();
        settings.IncludeExtensions = true;
        settings.IncludeUnsupported = true;

        OpenSummary(new MixedSample());
    }

    [RelayCommand]
    private void ShowExtensionsOptionWindow()
    {
        var settings = ResetEngineOptions();
        settings.IncludeExtensions = true;

        OpenSummary(new MixedSample());
    }

    /// <summary>
    ///     Restore the standard browsing defaults (static, events and extensions on) so data-set
    ///     and scenario windows render the same way regardless of which engine-option card ran before.
    /// </summary>
    private static DecompositionSettings ApplyDefaultOptions()
    {
        var settingsService = Host.GetService<ISettingsService>();
        settingsService.ResetDecompositionSettings();
        return settingsService.DecompositionSettings;
    }

    /// <summary>
    ///     Turn every decomposition flag off so an engine-option card starts from a known baseline
    ///     and shows the effect of the single flag it enables.
    /// </summary>
    private static DecompositionSettings ResetEngineOptions()
    {
        var settings = Host.GetService<ISettingsService>().DecompositionSettings;
        settings.IncludeRoot = false;
        settings.IncludeFields = false;
        settings.IncludeEvents = false;
        settings.IncludeUnsupported = false;
        settings.IncludePrivate = false;
        settings.IncludeStatic = false;
        settings.IncludeExtensions = false;
        return settings;
    }

    private static void OpenSummary(object value)
    {
        Host.GetService<IUiOrchestratorService>()
            .Decompose(value)
            .Show<DecompositionSummaryPage>();
    }

    private static void OpenSummary(IEnumerable values)
    {
        Host.GetService<IUiOrchestratorService>()
            .Decompose(values)
            .Show<DecompositionSummaryPage>();
    }
}
