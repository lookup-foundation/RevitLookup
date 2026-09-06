using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RevitLookup.UI.Framework.Presentation;

/// <summary>
///     Provides extension methods for <see cref="DependencyObject" /> and <see cref="ItemsControl" /> to walk the visual and logical trees.
/// </summary>
public static class VisualExtensions
{
    /// <param name="element">The element to walk the visual or logical tree from.</param>
    extension(DependencyObject element)
    {
        /// <summary>
        ///     Returns the nearest visual-tree ancestor of the specified type.
        /// </summary>
        /// <typeparam name="T">The <see cref="FrameworkElement" /> type to find.</typeparam>
        /// <returns>The nearest matching ancestor, or <see langword="null" /> if none is found.</returns>
        public T? FindVisualParent<T>() where T : FrameworkElement
        {
            var parentElement = (FrameworkElement?)VisualTreeHelper.GetParent(element);
            while (parentElement is not null)
            {
                if (parentElement is T parent)
                {
                    return parent;
                }

                parentElement = (FrameworkElement?)VisualTreeHelper.GetParent(parentElement);
            }

            return null;
        }

        /// <summary>
        ///     Returns the nearest visual-tree ancestor of the specified type and name.
        /// </summary>
        /// <typeparam name="T">The <see cref="FrameworkElement" /> type to find.</typeparam>
        /// <param name="name">The <see cref="FrameworkElement.Name" /> to match.</param>
        /// <returns>The nearest matching ancestor, or <see langword="null" /> if none is found.</returns>
        public T? FindVisualParent<T>(string name) where T : FrameworkElement
        {
            var parentElement = (FrameworkElement?)VisualTreeHelper.GetParent(element);
            while (parentElement is not null)
            {
                if (parentElement is T parent)
                {
                    if (parentElement.Name == name)
                    {
                        return parent;
                    }
                }

                parentElement = (FrameworkElement?)VisualTreeHelper.GetParent(parentElement);
            }

            return null;
        }

        /// <summary>
        ///     Returns the first visual-tree descendant of the specified type, searched depth-first.
        /// </summary>
        /// <typeparam name="T">The <see cref="Visual" /> type to find.</typeparam>
        /// <returns>The first matching descendant, or <see langword="null" /> if none is found.</returns>
        public T? FindVisualChild<T>() where T : Visual
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var childElement = (FrameworkElement?)VisualTreeHelper.GetChild(element, i);
                if (childElement is null)
                {
                    return null;
                }

                if (childElement is T child)
                {
                    return child;
                }

                var descendent = childElement.FindVisualChild<T>();
                if (descendent is not null)
                {
                    return descendent;
                }
            }

            return null;
        }

        /// <summary>
        ///     Returns the first visual-tree descendant of the specified type and name, searched depth-first.
        /// </summary>
        /// <typeparam name="T">The <see cref="Visual" /> type to find.</typeparam>
        /// <param name="name">The <see cref="FrameworkElement.Name" /> to match.</param>
        /// <returns>The first matching descendant, or <see langword="null" /> if none is found.</returns>
        public T? FindVisualChild<T>(string name) where T : Visual
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var childElement = (FrameworkElement?)VisualTreeHelper.GetChild(element, i);
                if (childElement is null)
                {
                    return null;
                }

                if (childElement is T child)
                {
                    if (childElement.Name == name)
                    {
                        return child;
                    }
                }

                var descendent = childElement.FindVisualChild<T>(name);
                if (descendent is not null)
                {
                    return descendent;
                }
            }

            return null;
        }

        /// <summary>
        ///     Returns the first logical-tree descendant of the specified type, searched depth-first.
        /// </summary>
        /// <typeparam name="T">The <see cref="Visual" /> type to find.</typeparam>
        /// <returns>The first matching descendant, or <see langword="null" /> if none is found.</returns>
        public T? FindLogicalChild<T>() where T : Visual
        {
            foreach (Visual child in LogicalTreeHelper.GetChildren(element))
            {
                if (child is T correctlyTyped)
                {
                    return correctlyTyped;
                }

                var descendent = child.FindLogicalChild<T>();
                if (descendent is not null)
                {
                    return descendent;
                }
            }

            return null;
        }

        /// <summary>
        ///     Returns the nearest logical-tree ancestor of the specified type.
        /// </summary>
        /// <typeparam name="T">The <see cref="DependencyObject" /> type to find.</typeparam>
        /// <returns>The nearest matching ancestor, or <see langword="null" /> if none is found.</returns>
        public T? FindLogicalParent<T>() where T : DependencyObject
        {
            var parentObject = LogicalTreeHelper.GetParent(element);
            while (parentObject is not null)
            {
                if (parentObject is T parent)
                {
                    return parent;
                }

                parentObject = LogicalTreeHelper.GetParent(parentObject);
            }

            return null;
        }
    }

    /// <param name="container">The items control to locate the item container in.</param>
    extension(ItemsControl container)
    {
        /// <summary>
        ///     Returns the realized container for the item at the specified index, generating and scrolling it into view if needed.
        /// </summary>
        /// <param name="index">The zero-based index of the item.</param>
        /// <returns>The item's container, or <see langword="null" /> if <paramref name="container" /> has no items or no container could be resolved.</returns>
        public DependencyObject? GetItemAtIndex(int index)
        {
            if (container.Items.Count == 0)
            {
                return null;
            }

            if (container is TreeViewItem { IsExpanded: false } viewItem)
            {
                viewItem.SetCurrentValue(TreeViewItem.IsExpandedProperty, true);
            }

            container.ApplyTemplate();
            var itemsPresenter = (ItemsPresenter)container.Template.FindName("ItemsHost", container);
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

            if (itemsPresenter is null)
            {
                return null;
            }

            var itemsHostPanel = (VirtualizingPanel)VisualTreeHelper.GetChild(itemsPresenter, 0);
            itemsHostPanel.BringIndexIntoViewPublic(index);
            return container.ItemContainerGenerator.ContainerFromIndex(index);
        }
    }
}
