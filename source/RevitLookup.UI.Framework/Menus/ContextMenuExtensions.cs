// Copyright (c) Lookup Foundation and Contributors
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// THIS PROGRAM IS PROVIDED "AS IS" AND WITH ALL FAULTS.
// NO IMPLIED WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE IS PROVIDED.
// THERE IS NO GUARANTEE THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using RevitLookup.UI.Framework.Presentation;

namespace RevitLookup.UI.Framework.Menus;

/// <summary>
///     Provides extension methods for <see cref="ContextMenu" /> and <see cref="MenuItem" /> to build a context menu fluently.
/// </summary>
public static class ContextMenuExtensions
{
    /// <param name="menu">The context menu to extend.</param>
    extension(ContextMenu menu)
    {
        /// <summary>
        ///     Appends a separator to the context menu.
        /// </summary>
        public void AddSeparator()
        {
            var separator = new Separator();
            menu.Items.Add(separator);
        }

        /// <summary>
        ///     Appends the shared "Label" resource item to the context menu with the specified header text.
        /// </summary>
        /// <param name="text">The header text to apply to the label.</param>
        /// <exception cref="InvalidOperationException">The "Label" resource is not found in the context menu's resources.</exception>
        public void AddLabel(string text)
        {
            var label = (MenuItem?)menu.Resources["Label"];
            if (label is null)
            {
                throw new InvalidOperationException("Resource \"Label\" not found");
            }

            label.Header = text;
            menu.Items.Add(label);
        }

        /// <summary>
        ///     Appends a new menu item to the context menu.
        /// </summary>
        /// <returns>The added <see cref="MenuItem" />.</returns>
        public MenuItem AddMenuItem()
        {
            var item = new Wpf.Ui.Controls.MenuItem();
            menu.Items.Add(item);

            return item;
        }

        /// <summary>
        ///     Appends a menu item resource to the context menu.
        /// </summary>
        /// <param name="resource">The key of the menu item resource in the context menu's resources.</param>
        /// <returns>The added <see cref="MenuItem" />.</returns>
        /// <exception cref="InvalidOperationException">No resource with the specified key is found in the context menu's resources.</exception>
        public MenuItem AddMenuItem(string resource)
        {
            var item = (MenuItem?)menu.Resources[resource];
            if (item is null)
            {
                throw new InvalidOperationException($"Resource \"{resource}\" not found");
            }

            menu.Items.Add(item);

            return item;
        }
    }

    /// <param name="item">The menu item to extend.</param>
    extension(MenuItem item)
    {
        /// <summary>
        ///     Appends a new child menu item to the menu item.
        /// </summary>
        /// <returns>The added child <see cref="MenuItem" />.</returns>
        public MenuItem AddMenuItem()
        {
            var child = new Wpf.Ui.Controls.MenuItem();
            item.Items.Add(child);

            return child;
        }

        /// <summary>
        ///     Appends a menu item resource as a child of the menu item.
        /// </summary>
        /// <param name="resource">The key of the menu item resource in the enclosing <see cref="ContextMenu" />'s resources.</param>
        /// <returns>The added child <see cref="MenuItem" />.</returns>
        /// <exception cref="InvalidOperationException">
        ///     The menu item has no enclosing <see cref="ContextMenu" />, or no resource with the specified key is found in its resources.
        /// </exception>
        public MenuItem AddMenuItem(string resource)
        {
            var child = (MenuItem?)item.FindLogicalParent<ContextMenu>()!.Resources[resource];
            if (child is null)
            {
                throw new InvalidOperationException($"Resource \"{resource}\" not found");
            }

            item.Items.Add(child);

            return child;
        }

        /// <summary>
        ///     Sets the command invoked when the menu item is clicked.
        /// </summary>
        /// <param name="command">The action to invoke.</param>
        /// <returns>The <see cref="MenuItem" /> for chaining.</returns>
        public MenuItem SetCommand(Action command)
        {
            item.Command = new RelayCommand(command);

            return item;
        }

        /// <summary>
        ///     Sets the command invoked when the menu item is clicked.
        /// </summary>
        /// <param name="command">The command to invoke.</param>
        /// <returns>The <see cref="MenuItem" /> for chaining.</returns>
        public MenuItem SetCommand(ICommand command)
        {
            item.Command = command;

            return item;
        }

        /// <summary>
        ///     Sets the asynchronous command invoked when the menu item is clicked.
        /// </summary>
        /// <param name="command">The asynchronous operation to invoke.</param>
        /// <returns>The <see cref="MenuItem" /> for chaining.</returns>
        public MenuItem SetCommand(Func<Task> command)
        {
            item.Command = new AsyncRelayCommand(command);

            return item;
        }

        /// <summary>
        ///     Sets the parameterized command invoked when the menu item is clicked.
        /// </summary>
        /// <typeparam name="T">The type of the command parameter.</typeparam>
        /// <param name="parameter">The value passed to <paramref name="command" /> when invoked.</param>
        /// <param name="command">The action to invoke.</param>
        /// <returns>The <see cref="MenuItem" /> for chaining.</returns>
        public MenuItem SetCommand<T>(T parameter, Action<T> command)
        {
            item.CommandParameter = parameter;
            item.Command = new RelayCommand<T>(command!);

            return item;
        }

        /// <summary>
        ///     Sets the parameterized asynchronous command invoked when the menu item is clicked.
        /// </summary>
        /// <typeparam name="T">The type of the command parameter.</typeparam>
        /// <param name="parameter">The value passed to <paramref name="command" /> when invoked.</param>
        /// <param name="command">The asynchronous operation to invoke.</param>
        /// <returns>The <see cref="MenuItem" /> for chaining.</returns>
        public MenuItem SetCommand<T>(T parameter, Func<T, Task> command)
        {
            item.CommandParameter = parameter;
            item.Command = new AsyncRelayCommand<T>(command!);

            return item;
        }

        /// <summary>
        ///     Registers a keyboard shortcut with modifier keys for the menu item's command on the enclosing context menu's placement target.
        /// </summary>
        /// <param name="modifiers">The modifier keys of the shortcut.</param>
        /// <param name="key">The key of the shortcut.</param>
        /// <returns>The <see cref="MenuItem" /> for chaining.</returns>
        /// <exception cref="InvalidOperationException">The menu item has no enclosing <see cref="ContextMenu" />.</exception>
        public MenuItem SetShortcut(ModifierKeys modifiers, Key key)
        {
            var inputGesture = new KeyGesture(key, modifiers);
            var menu = item.FindLogicalParent<ContextMenu>();
            if (menu is null)
            {
                throw new InvalidOperationException("Unable to find context menu");
            }

            menu.PlacementTarget.InputBindings.Add(new InputBinding(item.Command, inputGesture) { CommandParameter = item.CommandParameter });
            item.InputGestureText = inputGesture.GetDisplayStringForCulture(CultureInfo.InvariantCulture);

            return item;
        }

        /// <summary>
        ///     Registers a keyboard shortcut for the menu item's command on the enclosing context menu's placement target.
        /// </summary>
        /// <param name="key">The key of the shortcut.</param>
        /// <returns>The <see cref="MenuItem" /> for chaining.</returns>
        /// <exception cref="InvalidOperationException">The menu item has no enclosing <see cref="ContextMenu" />.</exception>
        public MenuItem SetShortcut(Key key)
        {
            var inputGesture = new KeyGesture(key);
            var menu = item.FindLogicalParent<ContextMenu>();
            if (menu is null)
            {
                throw new InvalidOperationException("Unable to find context menu");
            }

            menu.PlacementTarget.InputBindings.Add(new InputBinding(item.Command, inputGesture) { CommandParameter = item.CommandParameter });
            item.InputGestureText = inputGesture.GetDisplayStringForCulture(CultureInfo.InvariantCulture);

            return item;
        }

        /// <summary>
        ///     Sets the menu item's header text.
        /// </summary>
        /// <param name="text">The header text.</param>
        /// <returns>The <see cref="MenuItem" /> for chaining.</returns>
        public MenuItem SetHeader(string text)
        {
            item.Header = text;

            return item;
        }

        /// <summary>
        ///     Makes the menu item checkable and sets its checked state.
        /// </summary>
        /// <param name="state"><see langword="true" /> to check the menu item; otherwise, <see langword="false" />.</param>
        /// <returns>The <see cref="MenuItem" /> for chaining.</returns>
        public MenuItem SetChecked(bool state)
        {
            item.IsCheckable = true;
            item.IsChecked = state;

            return item;
        }

        /// <summary>
        ///     Sets the menu item's displayed gesture text without registering an input binding.
        /// </summary>
        /// <param name="key">The key to display as the gesture text.</param>
        /// <returns>The <see cref="MenuItem" /> for chaining.</returns>
        public MenuItem SetGestureText(Key key)
        {
            item.InputGestureText = new KeyGesture(key).GetDisplayStringForCulture(CultureInfo.InvariantCulture);

            return item;
        }

        /// <summary>
        ///     Sets whether the menu item is enabled.
        /// </summary>
        /// <param name="condition"><see langword="true" /> to enable the menu item; otherwise, <see langword="false" />.</param>
        /// <returns>The <see cref="MenuItem" /> for chaining.</returns>
        public MenuItem SetAvailability(bool condition)
        {
            item.SetCurrentValue(UIElement.IsEnabledProperty, condition);

            return item;
        }

        /// <summary>
        ///     Sets whether the enclosing context menu stays open after this menu item is clicked.
        /// </summary>
        /// <param name="condition"><see langword="true" /> to keep the context menu open; otherwise, <see langword="false" />.</param>
        /// <returns>The <see cref="MenuItem" /> for chaining.</returns>
        public MenuItem SetStaysOpenOnClick(bool condition)
        {
            item.SetCurrentValue(MenuItem.StaysOpenOnClickProperty, condition);

            return item;
        }
    }
}
