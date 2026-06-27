using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace RevitLookup.UI.Playground.Configuration;

/// <summary>
///     Application logging configuration
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
public static class LoggingConfiguration
{
    private const string LogTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

    public static TBuilder AddSerilogLoggingProvider<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var logger = CreateDefaultLogger();
        builder.Logging.AddSerilog(logger);

        return builder;
    }

    private static Logger CreateDefaultLogger()
    {
        return new LoggerConfiguration()
            .ConfigureSinks()
            .ConfigureMinimumLevel()
            .ConfigureEnrichers()
            .CreateLogger();
    }

    extension(LoggerConfiguration loggerConfiguration)
    {
        private LoggerConfiguration ConfigureSinks()
        {
            loggerConfiguration.WriteTo.Console(LogEventLevel.Information, outputTemplate: LogTemplate);

            if (Debugger.IsAttached)
            {
                loggerConfiguration.WriteTo.Debug(LogEventLevel.Debug, outputTemplate: LogTemplate);
            }

            return loggerConfiguration;
        }

        private LoggerConfiguration ConfigureMinimumLevel()
        {
            loggerConfiguration.MinimumLevel.Verbose();

            return loggerConfiguration;
        }

        private LoggerConfiguration ConfigureEnrichers()
        {
            return loggerConfiguration.Enrich.FromLogContext();
        }
    }
}