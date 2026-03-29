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

namespace RevitLookup.Abstractions.ObservableModels.Entries;

/// <summary>
///     Represents the observable model for the Revit INI entry.
/// </summary>
public sealed partial class ObservableIniEntry : ObservableValidator
{
    [ObservableProperty]
    [Required]
    [NotifyDataErrorInfo]
    public partial string Category { get; set; } = string.Empty;

    [ObservableProperty]
    [Required]
    [NotifyDataErrorInfo]
    public partial string Property { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Value { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? DefaultValue { get; set; }

    [ObservableProperty]
    public partial bool IsActive { get; set; }

    [ObservableProperty]
    public partial bool IsModified { get; private set; }
    public bool UserDefined { get; set; }

    public void Validate()
    {
        ValidateAllProperties();
    }

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