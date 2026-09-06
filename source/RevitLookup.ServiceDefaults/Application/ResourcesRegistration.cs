using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RevitLookup.Abstractions.Application;
using RevitLookup.ServiceDefaults.FileSystem;

namespace RevitLookup.ServiceDefaults.Application;

/// <summary>
///     Provides extension methods for <see cref="IHostApplicationBuilder" /> to bind the application's resource locations.
/// </summary>
[PublicAPI]
public static class ResourcesRegistration
{
    /// <param name="builder">The host application builder.</param>
    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        /// <summary>
        ///     Binds the roaming and local directories the application reads and writes its own files under to <see cref="ResourceLocationsOptions" />.
        /// </summary>
        /// <returns>The <see cref="TBuilder" /> for chaining.</returns>
        public TBuilder ConfigureResourceLocations()
        {
            builder.Services.AddOptions<ResourceLocationsOptions>().Configure<IHostEnvironment>((options, environment) =>
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version ??= new Version(1, 0);
                var majorVersion = version.Major.ToString();

                options.ApplicationDataDirectory = Environment
                    .GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    .AppendPath(environment.ApplicationName)
                    .AppendPath(majorVersion);

                options.LocalApplicationDataDirectory = Environment
                    .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                    .AppendPath(environment.ApplicationName)
                    .AppendPath(majorVersion);

                //Local directories
                options.DownloadsFolder = options.LocalApplicationDataDirectory.AppendPath("DownloadCache");

                //Roaming directories
                options.SettingsDirectory = options.ApplicationDataDirectory.AppendPath("Settings");

                //Roaming files
                options.ApplicationSettingsPath = options.SettingsDirectory.AppendPath("Application.json");
                options.DecompositionSettingsPath = options.SettingsDirectory.AppendPath("LookupEngine.json");
                options.VisualizationSettingsPath = options.SettingsDirectory.AppendPath("Visualization.json");
            }).ValidateDataAnnotations();

            return builder;
        }
    }
}
