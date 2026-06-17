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

using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

// ReSharper disable once CheckNamespace
namespace RevitLookup.UI.Framework.Controls;

/// <summary>
///     A compact, tinted status badge that pairs an optional icon with a short label.
///     The visual palette is driven by <see cref="Severity" />.
/// </summary>
public sealed class Chip : Control
{
    public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register(nameof(Severity), typeof(ChipSeverity), typeof(Chip), new PropertyMetadata(ChipSeverity.Neutral));
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(SymbolRegular), typeof(Chip), new PropertyMetadata(SymbolRegular.Empty));
    public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(Chip), new PropertyMetadata(14d));
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(Chip), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(Chip), new PropertyMetadata(new CornerRadius(4)));
    public static readonly DependencyProperty OutlinedProperty = DependencyProperty.Register(nameof(Outlined), typeof(bool), typeof(Chip), new PropertyMetadata(false));

    /// <summary>The palette applied to the chip background, icon and text.</summary>
    public ChipSeverity Severity
    {
        get => (ChipSeverity) GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    /// <summary>The leading icon. <see cref="SymbolRegular.Empty" /> hides the icon.</summary>
    public SymbolRegular Icon
    {
        get => (SymbolRegular) GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>The icon font size.</summary>
    public double IconSize
    {
        get => (double) GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    /// <summary>The label text. An empty value hides the text.</summary>
    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>The corner radius of the chip background.</summary>
    public CornerRadius CornerRadius
    {
        get => (CornerRadius) GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    ///     When <see langword="true"/>, the chip is drawn with a thin border and no fill.
    /// </summary>
    public bool Outlined
    {
        get => (bool) GetValue(OutlinedProperty);
        set => SetValue(OutlinedProperty, value);
    }
}