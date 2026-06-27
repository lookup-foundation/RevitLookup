using System.Windows;
using Microsoft.Extensions.Logging;
using RevitLookup.Common.Extensions;
using RevitLookup.UI.Playground.Views;

namespace RevitLookup.UI.Playground;

public sealed partial class App
{
    private void OnStartup(object sender, StartupEventArgs e)
    {
        try
        {
            Host.Start();
            var uiService = Host.GetService<IServiceProvider>();
            var playgroundView = uiService.CreateScopedFrameworkElement<PlaygroundView>();
            playgroundView.ShowDialog();
        }
        catch (Exception exception)
        {
            var logger = Host.GetService<ILogger<App>>();
            LogApplicationStartFailed(logger, exception);
            Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Host.Stop();
    }

    [LoggerMessage(LogLevel.Critical, "Application failed to start")]
    private static partial void LogApplicationStartFailed(ILogger<App> logger, Exception exception);
}