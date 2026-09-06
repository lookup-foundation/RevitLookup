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

using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitLookup.Abstractions.Tools;

/// <summary>
///     Represents the observable model for the Revit INI entry.
/// </summary>
public sealed partial class ObservableIniEntry : ObservableValidator
{
    /// <summary>
    ///     Gets or sets the INI section that contains this entry.
    /// </summary>
    [ObservableProperty]
    [Required]
    [NotifyDataErrorInfo]
    public partial string Category { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the INI property key.
    /// </summary>
    [ObservableProperty]
    [Required]
    [NotifyDataErrorInfo]
    public partial string Property { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the current value of the entry.
    /// </summary>
    [ObservableProperty]
    public partial string Value { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the value the entry reverts to when reset, or <see langword="null" /> when the entry has no default.
    /// </summary>
    [ObservableProperty]
    public partial string? DefaultValue { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the entry is written to the INI file.
    /// </summary>
    [ObservableProperty]
    public partial bool IsActive { get; set; }

    /// <summary>
    ///     Gets a value indicating whether <see cref="Value" /> differs from <see cref="DefaultValue" />.
    /// </summary>
    [ObservableProperty]
    public partial bool IsModified { get; private set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user changed <see cref="IsActive" /> from its initial state.
    /// </summary>
    public bool UserDefined { get; set; }

    /// <summary>
    ///     Validates <see cref="Category" /> and <see cref="Property" /> and populates their data errors.
    /// </summary>
    public void Validate()
    {
        ValidateAllProperties();
    }

    [UsedImplicitly(ImplicitUseTargetFlags.Itself, Reason = "Used by CommunityToolkit generator")]
    partial void OnIsActiveChanged(bool value)
    {
        UserDefined = true;
    }

    partial void OnValueChanged(string value)
    {
        IsModified = DefaultValue is not null && value != DefaultValue;
    }

    partial void OnDefaultValueChanged(string? value)
    {
        IsModified = value != Value;
    }

    /// <summary>
    ///     Creates a copy of the current entry with its <see cref="Category" />, <see cref="Property" />, and <see cref="Value" />.
    /// </summary>
    /// <returns>A new <see cref="ObservableIniEntry" /> with the same category, property, and value.</returns>
    public ObservableIniEntry Clone()
    {
        return new ObservableIniEntry
        {
            Category = Category,
            Property = Property,
            Value = Value
        };
    }
}
