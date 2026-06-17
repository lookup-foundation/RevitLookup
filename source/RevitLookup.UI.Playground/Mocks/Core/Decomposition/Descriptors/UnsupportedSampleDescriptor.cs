using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using RevitLookup.UI.Playground.Mocks.Models;

namespace RevitLookup.UI.Playground.Mocks.Core.Decomposition.Descriptors;

public sealed class UnsupportedSampleDescriptor : Descriptor, IDescriptorConfigurator
{
    public UnsupportedSampleDescriptor(UnsupportedSample sample)
    {
        Name = $"{sample.Host}:{sample.Port}";
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Extension("ExecuteNativeCall").NotSupported();
        configuration.Extension("GetRawHandle").NotSupported();
    }
}