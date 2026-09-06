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

using Autodesk.Revit.DB.ExternalService;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Decomposition.Descriptors;

/// <summary>
///     Represents the <see cref="ExternalService" /> exposed to LookupEngine.
/// </summary>
public sealed class ExternalServiceDescriptor : Descriptor, IDescriptorConfigurator
{
    private readonly ExternalService _service;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExternalServiceDescriptor" /> class.
    /// </summary>
    /// <param name="service">The external service to expose.</param>
    public ExternalServiceDescriptor(ExternalService service)
    {
        _service = service;
        Name = service.Name;
    }

    /// <inheritdoc />
    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(ExternalService.Dispose)).Disable();
        configuration.Member(nameof(ExternalService.GetServer)).Resolve(ResolveGetServer);
        return;

        IVariant ResolveGetServer()
        {
            var serverIds = _service.GetRegisteredServerIds();
            var variants = Variants.Values<IExternalServer>(_service.NumberOfServers);
            foreach (var serverId in serverIds)
            {
                variants.Add(_service.GetServer(serverId));
            }

            return variants.Consume();
        }
    }
}
