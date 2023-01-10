﻿// Copyright 2003-2022 by Autodesk, Inc.
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// 
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.

using RevitLookup.Core.ComponentModel;
using RevitLookup.Core.Objects;

namespace RevitLookup.Core.Utils;

public static class DescriptorUtils
{
    public static Descriptor FindSuitableDescriptor([CanBeNull] object obj)
    {
        var descriptor = DescriptorMap.FindDescriptor(obj);
        if (obj is null)
        {
            descriptor.Type = nameof(Object);
        }
        else
        {
            var type = obj.GetType();
            ValidateProperties(descriptor, type);
        }

        return descriptor;
    }

    public static Descriptor FindSuitableDescriptor([NotNull] object obj, Type type)
    {
        var descriptor = DescriptorMap.FindDescriptor(obj);
        ValidateProperties(descriptor, type);
        return descriptor;
    }

    private static void ValidateProperties(Descriptor descriptor, Type type)
    {
        descriptor.Type = MakeGenericTypeName(type);
        descriptor.Label ??= descriptor.Type;
    }

    private static string MakeGenericTypeName(Type type)
    {
        if (!type.IsGenericType) return type.Name;

        var typeName = type.Name;
        typeName = typeName.AsSpan(0, typeName.Length - 2).ToString();
        typeName += "<";
        var genericArguments = type.GetGenericArguments();
        for (var i = 0; i < genericArguments.Length; i++)
        {
            typeName += MakeGenericTypeName(genericArguments[i]);
            if (i < genericArguments.Length - 1) typeName += ", ";
        }

        typeName += ">";
        return typeName;
    }
}