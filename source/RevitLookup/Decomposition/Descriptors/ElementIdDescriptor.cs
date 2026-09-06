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

using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Decomposition.Descriptors;

/// <summary>
///     Represents the <see cref="Autodesk.Revit.DB.ElementId" /> exposed to LookupEngine.
/// </summary>
public sealed class ElementIdDescriptor : Descriptor, IDescriptorRedirector<Document>
{
    private readonly ElementId _elementId;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ElementIdDescriptor" /> class.
    /// </summary>
    /// <param name="elementId">The element identifier to expose.</param>
    public ElementIdDescriptor(ElementId elementId)
    {
        _elementId = elementId;
        Name = _elementId.ToString();
    }

    /// <inheritdoc />
    public bool TryRedirect(string target, Document context, out object result)
    {
        result = _elementId;
        if (target == nameof(Element.Id))
        {
            return false;
        }

        if (_elementId == ElementId.InvalidElementId)
        {
            return false;
        }

#if REVIT2024_OR_GREATER
        if (_elementId.Value is > -3000000 and < -2000000)
#else
        if (_elementId.IntegerValue is > -3000000 and < -2000000)
#endif
        {
            var element = Category.GetCategory(context, _elementId);
            if (element is null)
            {
                return false;
            }

            result = element;
            return true;
        }
        else
        {
            var element = _elementId.ToElement(context);
            if (element is null)
            {
                return false;
            }

            result = element;
            return true;
        }
    }
}
