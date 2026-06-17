using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RevitLookup.UI.Framework.Utils;

public static class VisualExtensions
{
    extension(DependencyObject element)
    {
        public T? FindVisualParent<T>() where T : FrameworkElement
        {
            var parentElement = (FrameworkElement?) VisualTreeHelper.GetParent(element);
            while (parentElement is not null)
            {
                if (parentElement is T parent) return parent;

                parentElement = (FrameworkElement?) VisualTreeHelper.GetParent(parentElement);
            }

            return null;
        }

        public T? FindVisualParent<T>(string name) where T : FrameworkElement
        {
            var parentElement = (FrameworkElement?) VisualTreeHelper.GetParent(element);
            while (parentElement is not null)
            {
                if (parentElement is T parent)
                {
                    if (parentElement.Name == name) return parent;
                }

                parentElement = (FrameworkElement?) VisualTreeHelper.GetParent(parentElement);
            }

            return null;
        }

        public T? FindVisualChild<T>() where T : Visual
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var childElement = (FrameworkElement?) VisualTreeHelper.GetChild(element, i);
                if (childElement is null) return null;

                if (childElement is T child) return child;

                var descendent = childElement.FindVisualChild<T>();
                if (descendent is not null) return descendent;
            }

            return null;
        }

        public T? FindVisualChild<T>(string name) where T : Visual
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var childElement = (FrameworkElement?) VisualTreeHelper.GetChild(element, i);
                if (childElement is null) return null;

                if (childElement is T child)
                {
                    if (childElement.Name == name) return child;
                }

                var descendent = childElement.FindVisualChild<T>(name);
                if (descendent is not null) return descendent;
            }

            return null;
        }

        public T? FindLogicalChild<T>() where T : Visual
        {
            foreach (Visual child in LogicalTreeHelper.GetChildren(element))
            {
                if (child is T correctlyTyped) return correctlyTyped;

                var descendent = child.FindLogicalChild<T>();
                if (descendent is not null) return descendent;
            }

            return null;
        }

        public T? FindLogicalParent<T>() where T : DependencyObject
        {
            var parentObject = LogicalTreeHelper.GetParent(element);
            while (parentObject is not null)
            {
                if (parentObject is T parent) return parent;
                parentObject = LogicalTreeHelper.GetParent(parentObject);
            }

            return null;
        }
    }

    public static DependencyObject? GetItemAtIndex(this ItemsControl container, int index)
    {
        if (container.Items.Count == 0) return null;

        if (container is TreeViewItem {IsExpanded: false} viewItem)
        {
            viewItem.SetCurrentValue(TreeViewItem.IsExpandedProperty, true);
        }

        container.ApplyTemplate();
        var itemsPresenter = (ItemsPresenter) container.Template.FindName("ItemsHost", container);
        if (itemsPresenter is not null)
        {
            itemsPresenter.ApplyTemplate();
        }
        else
        {
            itemsPresenter = container.FindVisualChild<ItemsPresenter>();
            if (itemsPresenter is null)
            {
                container.UpdateLayout();
                itemsPresenter = container.FindVisualChild<ItemsPresenter>();
            }
        }

        if (itemsPresenter is null) return null;

        var itemsHostPanel = (VirtualizingPanel) VisualTreeHelper.GetChild(itemsPresenter, 0);
        itemsHostPanel.BringIndexIntoViewPublic(index);
        return container.ItemContainerGenerator.ContainerFromIndex(index);
    }
}