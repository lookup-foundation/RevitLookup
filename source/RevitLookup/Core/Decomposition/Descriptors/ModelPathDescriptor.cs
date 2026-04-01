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

public sealed class ModelPathDescriptor : Descriptor, IDescriptorExtension
{
    private readonly ModelPath _modelPath;

    public ModelPathDescriptor(ModelPath modelPath)
    {
        _modelPath = modelPath;
        Name = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);
    }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define(nameof(ModelPathUtils.ConvertModelPathToUserVisiblePath)).Register(() => Variants.Value(ModelPathUtils.ConvertModelPathToUserVisiblePath(_modelPath)));
        manager.Define(nameof(TransmissionData.IsDocumentTransmitted)).Register(() => Variants.Value(TransmissionData.IsDocumentTransmitted(_modelPath)));
        manager.Define(nameof(TransmissionData.DocumentIsNotTransmitted)).Register(() => Variants.Value(TransmissionData.DocumentIsNotTransmitted(_modelPath)));
        manager.Define(nameof(TransmissionData.ReadTransmissionData)).Register(() => Variants.Value(TransmissionData.ReadTransmissionData(_modelPath)));
        manager.Define(nameof(WorksharingUtils.GetUserWorksetInfo)).Register(() => Variants.Value(WorksharingUtils.GetUserWorksetInfo(_modelPath)));
        manager.Define(nameof(ModelPathUtils.ConvertUserVisiblePathToModelPath)).AsNotSupported();
        manager.Define(nameof(ModelPathUtils.IsValidUserVisibleFullServerPath)).AsNotSupported();
        manager.Define(nameof(ModelPathUtils.ConvertCloudGUIDsToCloudPath)).AsNotSupported();
        manager.Define("CreateNewLocal").Map(nameof(WorksharingUtils.CreateNewLocal)).AsNotSupported();
    }
    
}