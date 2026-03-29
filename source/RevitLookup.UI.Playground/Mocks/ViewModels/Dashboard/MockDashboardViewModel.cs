using CommunityToolkit.Mvvm.Input;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Models.UserInterface;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Dashboard;
using RevitLookup.UI.Framework.Views.Decomposition;
using RevitLookup.UI.Framework.Views.Tools;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Dashboard;

[UsedImplicitly]
public sealed partial class MockDashboardViewModel : IDashboardViewModel
{
    private readonly IServiceProvider _serviceProvider;
    private readonly INavigationService _navigationService;
    private readonly INotificationService _notificationService;
    private readonly IVisualDecompositionService _visualDecompositionService;

    public MockDashboardViewModel(
        IServiceProvider serviceProvider,
        INavigationService navigationService,
        INotificationService notificationService,
        IVisualDecompositionService visualDecompositionService)
    {
        _serviceProvider = serviceProvider;
        _navigationService = navigationService;
        _notificationService = notificationService;
        _visualDecompositionService = visualDecompositionService;

        NavigationGroups =
        [
            new NavigationCardGroup
            {
                GroupName = "Workspace",
                Items =
                [
                    new NavigationCardItem
                    {
                        Title = "Active View",
                        Description = "Explore and analyze the model's visual representation",
                        Icon = SymbolRegular.Image24,
                        Command = NavigatePageCommand,
                        CommandParameter = "view"
                    },
                    new NavigationCardItem
                    {
                        Title = "Active Document",
                        Description = "Explore the open document, its structure and data",
                        Icon = SymbolRegular.Document24,
                        Command = NavigatePageCommand,
                        CommandParameter = "document"
                    },
                    new NavigationCardItem
                    {
                        Title = "Application",
                        Description = "Explore application-wide settings and global data",
                        Icon = SymbolRegular.Apps24,
                        Command = NavigatePageCommand,
                        CommandParameter = "application"
                    },
                    new NavigationCardItem
                    {
                        Title = "UI Application",
                        Description = "Explore an active session of the Revit user interface",
                        Icon = SymbolRegular.WindowApps24,
                        Command = NavigatePageCommand,
                        CommandParameter = "uiApplication"
                    },
                    new NavigationCardItem
                    {
                        Title = "UI Controlled Application",
                        Description = "Explore Revit add-in startup methods and events",
                        Icon = SymbolRegular.SquareHintApps24,
                        Command = NavigatePageCommand,
                        CommandParameter = "uiControlledApplication"
                    },
                    new NavigationCardItem
                    {
                        Title = "Database",
                        Description = "Explore the Revit model database and its elements",
                        Icon = SymbolRegular.Database24,
                        Command = NavigatePageCommand,
                        CommandParameter = "database"
                    },
                ]
            },
            new NavigationCardGroup
            {
                GroupName = "Interaction",
                Items =
                [
                    new NavigationCardItem
                    {
                        Title = "Selection",
                        Description = "Explore currently selected elements in the model",
                        Icon = SymbolRegular.SquareHint24,
                        Command = NavigatePageCommand,
                        CommandParameter = "selection"
                    },
                    new NavigationCardItem
                    {
                        Title = "Linked Element",
                        Description = "Select and explore an element linked from another model",
                        Icon = SymbolRegular.LinkSquare24,
                        Command = NavigatePageCommand,
                        CommandParameter = "linked"
                    },
                    new NavigationCardItem
                    {
                        Title = "Face",
                        Description = "Select and explore an element geometry face",
                        Icon = SymbolRegular.LayerDiagonal20,
                        Command = NavigatePageCommand,
                        CommandParameter = "face"
                    },
                    new NavigationCardItem
                    {
                        Title = "Edge",
                        Description = "Select and explore element geometry edges",
                        Icon = SymbolRegular.Line24,
                        Command = NavigatePageCommand,
                        CommandParameter = "edge"
                    },
                    new NavigationCardItem
                    {
                        Title = "Point",
                        Description = "Select and explore a specific location or coordinate",
                        Icon = SymbolRegular.Location24,
                        Command = NavigatePageCommand,
                        CommandParameter = "point"
                    },
                    new NavigationCardItem
                    {
                        Title = "Sub-Element",
                        Description = "Select and explore a selected element sub-element",
                        Icon = SymbolRegular.Subtitles24,
                        Command = NavigatePageCommand,
                        CommandParameter = "subElement"
                    },
                    new NavigationCardItem
                    {
                        Title = "Dependent Elements",
                        Description = "Explore child elements associated with the selection",
                        Icon = SymbolRegular.DataLine24,
                        Command = NavigatePageCommand,
                        CommandParameter = "dependents"
                    }
                ]
            },
            new NavigationCardGroup
            {
                GroupName = "Maintenance",
                Items =
                [
                    new NavigationCardItem
                    {
                        Title = "Component Manager",
                        Description = "Explore low-level visual components in Revit",
                        Icon = SymbolRegular.SlideTextMultiple32,
                        Command = NavigatePageCommand,
                        CommandParameter = "components"
                    },
                    new NavigationCardItem
                    {
                        Title = "Performance Advisor",
                        Description = "Explore performance issues in the open document",
                        Icon = SymbolRegular.HeartPulse24,
                        Command = NavigatePageCommand,
                        CommandParameter = "performance"
                    }
                ]
            },
            new NavigationCardGroup
            {
                GroupName = "Registry",
                Items =
                [
                    new NavigationCardItem
                    {
                        Title = "Updaters",
                        Description = "Explore all registered updaters in the session",
                        Icon = SymbolRegular.Whiteboard24,
                        Command = NavigatePageCommand,
                        CommandParameter = "updaters"
                    },
                    new NavigationCardItem
                    {
                        Title = "Schemas",
                        Description = "Explore Extensible Storage framework schemas",
                        Icon = SymbolRegular.Box24,
                        Command = NavigatePageCommand,
                        CommandParameter = "schemas"
                    },
                    new NavigationCardItem
                    {
                        Title = "Services",
                        Description = "Explore services extending the base Revit functionality",
                        Icon = SymbolRegular.WeatherCloudy24,
                        Command = NavigatePageCommand,
                        CommandParameter = "services"
                    }
                ]
            },
            new NavigationCardGroup
            {
                GroupName = "Units",
                Items =
                [
                    new NavigationCardItem
                    {
                        Title = "Built-In Parameters",
                        Description = "Explore predefined parameters available in Revit",
                        Icon = SymbolRegular.LeafOne24,
                        Command = OpenDialogCommand,
                        CommandParameter = "parameters"
                    },
                    new NavigationCardItem
                    {
                        Title = "Built-In Categories",
                        Description = "Explore predefined categories available in Revit",
                        Icon = SymbolRegular.LeafTwo24,
                        Command = OpenDialogCommand,
                        CommandParameter = "categories"
                    },
                    new NavigationCardItem
                    {
                        Title = "Forge Schema",
                        Description = "Explore Forge schema definitions used in Revit",
                        Icon = SymbolRegular.LeafThree24,
                        Command = OpenDialogCommand,
                        CommandParameter = "forge"
                    }
                ]
            },
            new NavigationCardGroup
            {
                GroupName = "Tools",
                Items =
                [
                    new NavigationCardItem
                    {
                        Title = "Search elements",
                        Description = "Search for specific elements in the model",
                        Icon = SymbolRegular.SlideSearch24,
                        Command = OpenDialogCommand,
                        CommandParameter = "search"
                    },
                    new NavigationCardItem
                    {
                        Title = "Event monitor",
                        Description = "Monitor all incoming events in a Revit session",
                        Icon = SymbolRegular.DesktopPulse24,
                        Command = NavigatePageCommand,
                        CommandParameter = "events"
                    },
                    new NavigationCardItem
                    {
                        Title = "Revit settings",
                        Description = "Inspect configuration and settings available in Revit",
                        Icon = SymbolRegular.LauncherSettings24,
                        Command = NavigatePageCommand,
                        CommandParameter = "revitSettings"
                    },
                    new NavigationCardItem
                    {
                        Title = "Modules",
                        Description = "Inspect the dynamic link libraries (DLLs) that Revit uses",
                        Icon = SymbolRegular.BroadActivityFeed24,
                        Command = OpenDialogCommand,
                        CommandParameter = "modules"
                    }
                ]
            }
        ];
    }

    public List<NavigationCardGroup> NavigationGroups { get; }

    [RelayCommand]
    private async Task NavigatePage(string? parameter)
    {
        switch (parameter)
        {
            case "view":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.View);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "document":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.Document);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "application":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.Application);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "uiApplication":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.UiApplication);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "uiControlledApplication":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.UiControlledApplication);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "database":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.Database);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "dependents":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.DependentElements);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "selection":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.Selection);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "linked":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.LinkedElement);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "face":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.Face);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "edge":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.Edge);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "point":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.Point);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "subElement":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.SubElement);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "components":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.ComponentManager);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "performance":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.PerformanceAdviser);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "updaters":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.UpdaterRegistry);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "services":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.Services);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "schemas":
                await _visualDecompositionService.VisualizeDecompositionAsync(KnownDecompositionObject.Schemas);
                _navigationService.Navigate(typeof(DecompositionSummaryPage));
                break;
            case "events":
                _navigationService.Navigate(typeof(EventsSummaryPage));
                break;
            case "revitSettings":
                _navigationService.NavigateWithHierarchy(typeof(RevitSettingsPage));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(parameter), parameter);
        }
    }

    [RelayCommand]
    private async Task OpenDialog(string parameter)
    {
        try
        {
            switch (parameter)
            {
                case "parameters":
                    var unitsDialog = _serviceProvider.GetRequiredService<UnitsDialog>();
                    await unitsDialog.ShowParametersDialogAsync();
                    return;
                case "categories":
                    unitsDialog = _serviceProvider.GetRequiredService<UnitsDialog>();
                    await unitsDialog.ShowCategoriesDialogAsync();
                    return;
                case "forge":
                    unitsDialog = _serviceProvider.GetRequiredService<UnitsDialog>();
                    await unitsDialog.ShowForgeSchemaDialogAsync();
                    return;
                case "search":
                    var searchDialog = _serviceProvider.GetRequiredService<SearchElementsDialog>();
                    await searchDialog.ShowAsync();
                    return;
                case "modules":
                    var modulesDialog = _serviceProvider.GetRequiredService<ModulesDialog>();
                    await modulesDialog.ShowAsync();
                    return;
            }
        }
        catch (Exception exception)
        {
            _notificationService.ShowError("Failed to open dialog", exception);
        }
    }
}