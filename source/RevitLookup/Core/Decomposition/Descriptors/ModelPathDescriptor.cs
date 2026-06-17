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

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class ModelPathDescriptor : Descriptor, IDescriptorConfigurator
{
    private readonly ModelPath _modelPath;

    public ModelPathDescriptor(ModelPath modelPath)
    {
        _modelPath = modelPath;
        Name = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Extension(nameof(ModelPathUtils.ConvertModelPathToUserVisiblePath)).Register(() => ModelPathUtils.ConvertModelPathToUserVisiblePath(_modelPath));
        configuration.Extension(nameof(TransmissionData.IsDocumentTransmitted)).Register(() => TransmissionData.IsDocumentTransmitted(_modelPath));
        configuration.Extension(nameof(TransmissionData.DocumentIsNotTransmitted)).Register(() => TransmissionData.DocumentIsNotTransmitted(_modelPath));
        configuration.Extension(nameof(TransmissionData.ReadTransmissionData)).Register(() => TransmissionData.ReadTransmissionData(_modelPath));
        configuration.Extension(nameof(WorksharingUtils.GetUserWorksetInfo)).Register(() => WorksharingUtils.GetUserWorksetInfo(_modelPath));
        configuration.Extension(nameof(ModelPathUtils.ConvertUserVisiblePathToModelPath)).NotSupported();
        configuration.Extension(nameof(ModelPathUtils.IsValidUserVisibleFullServerPath)).NotSupported();
        configuration.Extension(nameof(ModelPathUtils.ConvertCloudGUIDsToCloudPath)).NotSupported();
        configuration.Extension("CreateNewLocal").Map(nameof(WorksharingUtils.CreateNewLocal)).NotSupported();
    }
}