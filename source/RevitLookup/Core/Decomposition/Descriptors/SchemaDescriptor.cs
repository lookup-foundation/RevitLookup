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

using Autodesk.Revit.DB.ExtensibleStorage;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using RevitLookup.Utils;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class SchemaDescriptor : Descriptor, IDescriptorConfigurator, IDescriptorConfigurator<Document>
{
    private readonly Schema _schema;

    public SchemaDescriptor(Schema schema)
    {
        _schema = schema;
        Name = schema.SchemaName;
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(Schema.ListFields)).Resolve(ResolveListFields);
        return;

        IVariant ResolveListFields()
        {
            using (_schema.GrantAccess())
            {
                return Variants.Value(_schema.ListFields());
            }
        }
    }

    void IDescriptorConfigurator<Document>.Configure(IMemberConfigurator<Document> configuration)
    {
        configuration.Extension("GetElements").Register(context => context.CollectElements()
            .WithExtensibleStorage(_schema.GUID)
            .ToElements());
    }
}