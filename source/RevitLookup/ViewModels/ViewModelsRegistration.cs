using Microsoft.Extensions.DependencyInjection;

namespace RevitLookup.ViewModels;

/// <summary>
///     Provides extension methods for <see cref="IServiceCollection" /> to register view models.
/// </summary>
public static class ViewModelsRegistration
{
    /// <param name="services">The service collection to extend.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds all view model types from the current assembly to the specified <see cref="IServiceCollection" />.
        /// </summary>
        public void AddViewModels()
        {
            services.Scan(selector => selector.FromAssemblyOf<Application>()
                .AddClasses(filter => filter.Where(static type => type.Name.EndsWith("ViewModel")))
                .AsImplementedInterfaces(static type => type.Name.EndsWith("ViewModel"))
                .WithScopedLifetime());
        }
    }
}
