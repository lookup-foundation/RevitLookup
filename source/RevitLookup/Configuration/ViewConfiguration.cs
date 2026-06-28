using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using RevitLookup.UI.Framework;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace RevitLookup.Configuration;

public static class ViewConfiguration
{
    public static void AddViews(this IServiceCollection services)
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