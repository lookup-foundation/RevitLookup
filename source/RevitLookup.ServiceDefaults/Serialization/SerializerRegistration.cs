using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RevitLookup.ServiceDefaults.Serialization;

/// <summary>
///     Provides extension methods for <see cref="IHostApplicationBuilder" /> to configure JSON serialization.
/// </summary>
[PublicAPI]
public static class SerializerRegistration
{
    /// <param name="builder">The host application builder.</param>
    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        /// <summary>
        ///     Configures the JSON serializer options the application serializes through.
        /// </summary>
        /// <returns>The <see cref="TBuilder" /> for chaining.</returns>
        public TBuilder ConfigureJsonSerializerDefaults()
        {
            builder.Services.Configure<JsonSerializerOptions>(options =>
            {
                options.WriteIndented = true;
                options.PropertyNameCaseInsensitive = true;
                options.DefaultIgnoreCondition = builder.Environment.IsDevelopment() ? JsonIgnoreCondition.Never : JsonIgnoreCondition.WhenWritingNull;
                options.Converters.Add(new JsonStringEnumConverter());
            });

            return builder;
        }
    }
}
