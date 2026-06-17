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
using Autodesk.Revit.DB.ExtensibleStorage;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using RevitLookup.Utils;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class EntityDescriptor(Entity entity) : Descriptor, IDescriptorConfigurator
{
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(Entity.Get))
            .When(parameters => parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
            .Resolve(ResolveGetByField);
        configuration.Member(nameof(Entity.Get))
            .When(parameters => parameters.Length == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(ForgeTypeId))
            .Resolve(ResolveGetByFieldForge);
        return;

        IVariant ResolveGetByField()
        {
            using (entity.Schema.GrantAccess())
            {
                var fields = entity.Schema.ListFields();
                var variants = Variants.Values<object>(fields.Count);
                foreach (var field in fields)
                {
                    //for double we always need UnitTypeId, so we can not see these fields when we use the overload with fieldName only
                    if (field.ValueType == typeof(double) || field.KeyType == typeof(double)) continue;

                    var method = entity.GetType().GetMethod(nameof(Entity.Get), [typeof(Field)])!;
                    var genericMethod = MakeGenericInvoker(field, method);
                    variants.Add(genericMethod.Invoke(entity, [field]), field.FieldName);
                }

                return variants.Consume();
            }
        }

        IVariant ResolveGetByFieldForge()
        {
            using (entity.Schema.GrantAccess())
            {
                var fields = entity.Schema.ListFields();
                var variants = Variants.Values<object>(fields.Count);
                foreach (var field in fields)
                {
                    var forgeTypeId = field.GetSpecTypeId();
                    var unit = GetValidUnit(forgeTypeId);
                    var method = entity.GetType().GetMethod(nameof(Entity.Get), [typeof(Field), typeof(ForgeTypeId)])!;
                    var genericMethod = MakeGenericInvoker(field, method);
                    variants.Add(genericMethod.Invoke(entity, [field, unit]), field.FieldName);
                }

                return variants.Consume();
            }
        }
    }

    private static ForgeTypeId GetValidUnit(ForgeTypeId forgeTypeId)
    {
#if REVIT2022_OR_GREATER
        var isMeasurableSpec = UnitUtils.IsMeasurableSpec(forgeTypeId);
#else
        var isMeasurableSpec = false;
        try
        {
            if (UnitUtils.IsSpec(forgeTypeId))
            {
                UnitUtils.GetValidUnits(forgeTypeId);
                isMeasurableSpec = true;
            }
        }
        catch
        {
            // ignored
        }
#endif

        return isMeasurableSpec ? UnitUtils.GetValidUnits(forgeTypeId).First() : UnitTypeId.Custom;
    }

    private static MethodInfo MakeGenericInvoker(Field field, MethodInfo invoker)
    {
        var containerType = field.ContainerType switch
        {
            ContainerType.Simple => field.ValueType,
            ContainerType.Array => typeof(IList<>).MakeGenericType(field.ValueType),
            ContainerType.Map => typeof(IDictionary<,>).MakeGenericType(field.KeyType, field.ValueType),
            _ => throw new ArgumentOutOfRangeException()
        };

        return invoker.MakeGenericMethod(containerType);
    }
}