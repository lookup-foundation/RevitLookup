using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModularPipelines.Context;
using ModularPipelines.Engine;

namespace Build.Komac;

public static class KomacExtensions
{
    [ModuleInitializer]
    public static void RegisterKomacContext()
    {
        ModularPipelinesContextRegistry.RegisterContext(collection => collection.RegisterKomacContext());
    }

    private static IServiceCollection RegisterKomacContext(this IServiceCollection services)
    {
        services.TryAddScoped<Komac>();
        return services;
    }

    public static Komac Komac(this IPipelineContext context) => context.Services.Get<Komac>();
}
