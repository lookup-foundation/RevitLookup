using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModularPipelines.Context;
using ModularPipelines.Engine;

namespace Build.Azure;

/// <summary>
///     Provides extension methods for registering and resolving the <see cref="AzureSignTool" /> pipeline service.
/// </summary>
public static class AzureSignToolExtensions
{
    /// <summary>
    ///     Registers the <see cref="AzureSignTool" /> service with the ModularPipelines context registry.
    /// </summary>
    [ModuleInitializer]
    public static void RegisterAzureSignToolContext()
    {
        ModularPipelinesContextRegistry.RegisterContext(collection => collection.RegisterAzureSignToolContext());
    }

    extension(IServiceCollection services)
    {
        private IServiceCollection RegisterAzureSignToolContext()
        {
            services.TryAddScoped<AzureSignTool>();
            return services;
        }
    }

    /// <param name="context">The pipeline context to resolve the <see cref="AzureSignTool" /> service from.</param>
    extension(IPipelineContext context)
    {
        /// <summary>
        ///     Gets the <see cref="AzureSignTool" /> service registered for the current pipeline.
        /// </summary>
        /// <returns>The <see cref="AzureSignTool" /> instance resolved from the pipeline's service provider.</returns>
        public AzureSignTool Azure()
        {
            return context.Services.Get<AzureSignTool>();
        }
    }
}
