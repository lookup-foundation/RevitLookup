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
/// public partial class Class(ILogger&lt;Class&gt; logger)
/// {
///     private void Execute()
///     {
///         LogMessage(logger);
///     }
///
///     [LoggerMessage(LogLevel.Information, "Message")]
///     private static partial void LogMessage(ILogger&lt;Class&gt; logger);
/// }
/// </code>
/// </example>
public static partial class LoggerConfiguration
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
            .ConfigureSinks(environment)
            .ConfigureMinimumLevel(environment)
            .ConfigureEnrichers()
            .CreateLogger();
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        var exception = (Exception) args.ExceptionObject;
        var logger = Host.GetService<ILogger<AppDomain>>();
        LogDomainUnhandledException(logger, exception);
    }

    extension(Serilog.LoggerConfiguration loggerConfiguration)
    {
        private Serilog.LoggerConfiguration ConfigureSinks(IHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                if (Debugger.IsAttached)
                {
                    loggerConfiguration.WriteTo.Debug(LogEventLevel.Debug, outputTemplate: LogTemplate);
                    return loggerConfiguration;
                }
            }

            loggerConfiguration.WriteTo.Console(LogEventLevel.Information, outputTemplate: LogTemplate);
            loggerConfiguration.WriteTo.RevitJournal(RevitContext.UiApplication, false, LogTemplate, LogEventLevel.Error);

            return loggerConfiguration;
        }

        private Serilog.LoggerConfiguration ConfigureMinimumLevel(IHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                loggerConfiguration.MinimumLevel.Verbose();
                if (Debugger.IsAttached)
                {
                    return loggerConfiguration;
                }
            }
            else
            {
                loggerConfiguration.MinimumLevel.Information();
            }

            loggerConfiguration.MinimumLevel.Override("Microsoft.Extensions.Http.DefaultHttpClientFactory", LogEventLevel.Warning);

            return loggerConfiguration;
        }

        private Serilog.LoggerConfiguration ConfigureEnrichers()
        {
            return loggerConfiguration.Enrich.FromLogContext();
        }
    }

    [LoggerMessage(LogLevel.Critical, "Domain unhandled exception")]
    private static partial void LogDomainUnhandledException(ILogger<AppDomain> logger, Exception exception);
}