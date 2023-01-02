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

using System.Reflection;
using Autodesk.Revit.DB;
using RevitLookup.Core.Descriptors.Utils;

namespace RevitLookup.Core.Descriptors;

public sealed class BoolDescriptor : Descriptor
{
    public BoolDescriptor(bool value)
    {
        Label = value ? "true" : "false";
    }
}

public sealed class StringDescriptor : Descriptor
{
    private readonly string _value;

    public StringDescriptor(string value)
    {
        _value = value;
        Label = value;
    }

    public override bool TryInvoke(out IReadOnlyList<Descriptor> members)
    {
        members = HandlerUtils.HandleMethods(this, _value);
        return true;
    }

    public override bool TryInvoke(string methodName, ParameterInfo[] args, out object result)
    {
        if (methodName == nameof(object.Equals))
        {
            result = true;
            return true;
        }

        return base.TryInvoke(methodName, args, out result);
    }
}

public sealed class IntDescriptor : Descriptor
{
    public IntDescriptor(int value)
    {
        Label = value.ToString();
    }
}

public sealed class ExceptionDescriptor : Descriptor
{
    public ExceptionDescriptor(Exception value)
    {
        if (value.InnerException is null)
            Label = value.Message;
        else
            Label = string.IsNullOrEmpty(value.InnerException.Message) ? value.Message : value.InnerException.Message;
    }
}

public sealed class ElementDescriptor : Descriptor
{
    public ElementDescriptor(Element value)
    {
        var name = value.Name == string.Empty ? "<empty>" : value.Name;
        Label = $"{name}, ID{value.Id}";
    }
}

public sealed class ObjectDescriptor : Descriptor
{
    public ObjectDescriptor()
    {
    }

    public ObjectDescriptor(object value)
    {
        Label = value.ToString();
    }
}