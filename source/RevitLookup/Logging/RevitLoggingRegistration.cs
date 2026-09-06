using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nice3point.Revit.Logging;

namespace RevitLookup.Logging;

/// <summary>
///     Revit-specific logging configuration.
/// </summary>
public static class RevitLoggingRegistration
{
    /// <param name="builder">The host application builder to configure.</param>
    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        /// <summary>
        ///     Seeds the log levels the add-in runs on, adds the Revit journal logging provider, and silences the WPF resource dictionary traces.
        /// </summary>
        /// <returns>The <see cref="TBuilder" /> for chaining.</returns>
        public TBuilder AddRevitLogging()
        {
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Logging:LogLevel:Default"] = builder.Environment.IsDevelopment() ? nameof(LogLevel.Debug) : nameof(LogLevel.Information),
                ["Logging:LogLevel:Microsoft.Extensions.Http.DefaultHttpClientFactory"] = nameof(LogLevel.Warning),
                ["Logging:RevitJournal:LogLevel:Default"] = nameof(LogLevel.Error)
            });

            builder.Logging.AddRevitJournal(RevitApiContext.Application, options => options.ApplicationName = nameof(RevitLookup));

            PresentationTraceSources.ResourceDictionarySource.Switch.Level = SourceLevels.Critical;

            return builder;
        }
    }
}
