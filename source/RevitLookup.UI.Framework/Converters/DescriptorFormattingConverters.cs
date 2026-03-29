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

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using RevitLookup.Abstractions.ObservableModels.Decomposition;

namespace RevitLookup.UI.Framework.Converters;

public static class DescriptorFormattingConverters
{
    public static IValueConverter ObjectDisplayText { get; } = new ObjectConverter();
    public static IValueConverter MemberDisplayText { get; } = new MemberConverter();

    private abstract class DescriptorLabelConverterBase : IValueConverter
    {
        public abstract object Convert(object? value, Type targetType, object? parameter, CultureInfo culture);

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        protected static bool TryConvertInvalidNames(object? value, [MaybeNullWhen(false)] out string result)
        {
            result = value switch
            {
                null => "<null>",
                string {Length: 0} => "<empty>",
                _ => null
            };

            return result is not null;
        }
    }

    private sealed class ObjectConverter : DescriptorLabelConverterBase
    {
        public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var member = (ObservableDecomposedObject) value!;

            if (TryConvertInvalidNames(member.RawValue, out var displayText))
            {
                return displayText;
            }

            return CreateDisplayText(member.Name, member.Description);
        }

        private static string CreateDisplayText(string name, string? description)
        {
            return string.IsNullOrEmpty(description) ? name : description!;
        }
    }

    private sealed class MemberConverter : DescriptorLabelConverterBase
    {
        public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var member = (ObservableDecomposedMember) value!;

            if (TryConvertInvalidNames(member.Value.RawValue, out var displayText))
            {
                return displayText;
            }

            return CreateDisplayText(member.Value.Name, member.Value.Description);
        }

        private static string CreateDisplayText(string name, string? description)
        {
            if (string.IsNullOrEmpty(description)) return name;
            if (description!.EndsWith(name, StringComparison.OrdinalIgnoreCase)) return description;

            return $"{description}: {name}";
        }
    }
}