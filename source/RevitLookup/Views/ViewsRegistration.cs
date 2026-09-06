using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using RevitLookup.UI.Framework;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace RevitLookup.Views;

/// <summary>
///     Provides registration of RevitLookup's windows, dialogs, and pages.
/// </summary>
public static class ViewsRegistration
{
    /// <param name="services">The service collection to add the views to.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds RevitLookup's windows, dialogs, and pages to the specified <see cref="IServiceCollection" />.
        /// </summary>
        public void AddViews()
        {
            services.Scan(selector => selector.FromAssemblyOf<App>()
                .AddClasses(filter => filter.AssignableTo<FluentWindow>()).AsSelf().WithScopedLifetime()
                .AddClasses(filter => filter.AssignableTo<ContentDialog>()).AsSelf().WithTransientLifetime()
                .AddClasses(filter =>
                {
                    filter.AssignableTo<Page>();
                    filter.Where(static type => typeof(INavigableView<object>).IsAssignableFrom(type));
                }).AsSelf().WithScopedLifetime()
                .AddClasses(filter =>
                {
                    filter.AssignableTo<Page>();
                    filter.Where(static type => !typeof(INavigableView<object>).IsAssignableFrom(type));
                }).AsSelf().WithTransientLifetime());
        }
    }
}
