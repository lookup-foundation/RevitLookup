using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace RevitLookup.Configuration;

/// <summary>
///     Application logging configuration.
/// </summary>
/// <example>
/// <code lang="csharp">
/// public class Class(ILogger&lt;Class&gt; logger)
/// {
///     private void Execute()
///     {
///         logger.LogInformation("Message");
///     }
/// }
/// </code>
/// </example>
public static class LoggerConfiguration
{
    private const string LogTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

    public static TBuilder AddSerilogLoggingProvider<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var logger = CreateDefaultLogger(builder.Environment);
        builder.Logging.AddSerilog(logger);

        PresentationTraceSources.ResourceDictionarySource.Switch.Level = SourceLevels.Critical;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        return builder;
    }

    private static Logger CreateDefaultLogger(IHostEnvironment environment)
    {
        return new Serilog.LoggerConfiguration()
            .ConfigureSinks()
            .ConfigureMinimumLevel(environment)
            .ConfigureEnrichers(environment)
            .CreateLogger();
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        var exception = (Exception) args.ExceptionObject;
        var logger = Host.GetService<ILogger<AppDomain>>();
        logger.LogCritical(exception, "Domain unhandled exception");
    }

    extension(Serilog.LoggerConfiguration loggerConfiguration)
    {
        private Serilog.LoggerConfiguration ConfigureSinks()
        {
            loggerConfiguration.WriteTo.Console(LogEventLevel.Information, outputTemplate: LogTemplate);

            if (Debugger.IsAttached)
            {
                loggerConfiguration.WriteTo.Debug(LogEventLevel.Debug, outputTemplate: LogTemplate);
                return loggerConfiguration;
            }

            loggerConfiguration.WriteTo.RevitJournal(RevitContext.UiApplication, false, LogTemplate, LogEventLevel.Error);

            return loggerConfiguration;
        }

        private Serilog.LoggerConfiguration ConfigureMinimumLevel(IHostEnvironment environment)
        {
            loggerConfiguration.MinimumLevel.Verbose();
            if (Debugger.IsAttached) return loggerConfiguration;

            if (environment.IsDevelopment())
            {
                loggerConfiguration.MinimumLevel.Override("System.Net.Http.HttpClient.OpenSearchClient.LogicalHandler", LogEventLevel.Information);
                loggerConfiguration.MinimumLevel.Override("System.Net.Http.HttpClient.OpenSearchClient.ClientHandler", LogEventLevel.Information);
            }
            else
            {
                loggerConfiguration.MinimumLevel.Override("System.Net.Http.HttpClient.OpenSearchClient.LogicalHandler", LogEventLevel.Warning);
                loggerConfiguration.MinimumLevel.Override("System.Net.Http.HttpClient.OpenSearchClient.ClientHandler", LogEventLevel.Warning);
            }

            loggerConfiguration.MinimumLevel.Override("Microsoft.Extensions.Http.DefaultHttpClientFactory", LogEventLevel.Warning);

            return loggerConfiguration;
        }

        private Serilog.LoggerConfiguration ConfigureEnrichers(IHostEnvironment environment)
        {
            return loggerConfiguration.Enrich.FromLogContext();
        }
    }
}