using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Services.Appearance;
using RevitLookup.Abstractions.Services.Application;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.Configuration;
using RevitLookup.ServiceDefaults;
using RevitLookup.Services.Appearance;
using RevitLookup.Services.Application;
using RevitLookup.Services.Decomposition;
using RevitLookup.Services.Settings;
using RevitLookup.UI.Framework.Services;
using RevitLookup.UI.Framework.Services.Presentation;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using SoftwareUpdateService = RevitLookup.Services.Settings.SoftwareUpdateService;

namespace RevitLookup;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes
/// </summary>
public static class Host
{
    private static IHost? _host;

    /// <summary>
    ///     Starts the host and configures the application's services
    /// </summary>
    public static async Task StartAsync()
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            DisableDefaults = true,
#if ENVIRONMENT_PRODUCTION
            EnvironmentName = Environments.Production
#else
            EnvironmentName = Environments.Development
#endif
        });

        //Logging
        builder.Logging.ClearProviders();
        builder.AddSerilogLoggingProvider();

        //Configuration
        builder.AddServiceDefaults();
        builder.ConfigureHttpClients();

        //Frontend services
        builder.Services.AddScoped<INavigationViewPageProvider, DependencyInjectionNavigationViewPageProvider>();
        builder.Services.AddScoped<INavigationService, NavigationService>();
        builder.Services.AddScoped<IContentDialogService, ContentDialogService>();
        builder.Services.AddScoped<ISnackbarService, SnackbarService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<IWindowIntercomService, WindowIntercomService>();

        //MVVM services
        builder.Services.AddViews();
        builder.Services.AddViewModels();

        //Application services
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<ISoftwareUpdateService, SoftwareUpdateService>();
        builder.Services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        builder.Services.AddSingleton<RevitRibbonService>();
        builder.Services.AddHostedService<HostBackgroundService>();

        //Composer services
        builder.Services.AddScoped<IDecompositionService, DecompositionService>();
        builder.Services.AddScoped<IVisualDecompositionService, VisualDecompositionService>();
        builder.Services.AddScoped<IDecompositionSearchService, DecompositionSearchService>();
        builder.Services.AddTransient<IUiOrchestratorService, UiOrchestratorService>();

        //Revit tools services
        builder.Services.AddTransient<EventsMonitoringService>();

        _host = builder.Build();
        await _host.StartAsync();
    }

    /// <summary>
    ///     Stops the host and handle <see cref="IHostedService"/> services
    /// </summary>
    public static async Task StopAsync()
    {
        if (_host is null) return;

        await _host.StopAsync();
    }

    /// <summary>
    ///     Get service of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of service object to get</typeparam>
    /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T"/></exception>
    public static T GetService<T>() where T : class
    {
        return _host!.Services.GetRequiredService<T>();
    }
}