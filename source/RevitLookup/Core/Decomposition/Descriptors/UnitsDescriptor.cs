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

using System.Reflection;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class UnitsDescriptor(Autodesk.Revit.DB.Units units) : Descriptor, IDescriptorResolver, IDescriptorExtension
{
    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Autodesk.Revit.DB.Units.GetFormatOptions) => ResolveGetFormatOptions,
            _ => null
        };

        IVariant ResolveGetFormatOptions()
        {
            var specProperties = typeof(SpecTypeId).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            var values = Variants.Values<FormatOptions>(specProperties.Length);

            foreach (var property in specProperties)
            {
                var propertyValue = (ForgeTypeId) property.GetValue(null)!;
                var formatOptions = units.GetFormatOptions(propertyValue);
                values.Add(formatOptions, propertyValue.TypeId);
            }

            return values.Consume();
        }
    }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define(nameof(UnitFormatUtils.Format)).AsNotSupported();
        manager.Define(nameof(UnitFormatUtils.TryParse)).AsNotSupported();
    }
}