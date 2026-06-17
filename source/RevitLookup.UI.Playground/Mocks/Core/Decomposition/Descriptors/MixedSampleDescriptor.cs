using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using RevitLookup.UI.Playground.Mocks.Models;

namespace RevitLookup.UI.Playground.Mocks.Core.Decomposition.Descriptors;

public sealed class MixedSampleDescriptor : Descriptor, IDescriptorConfigurator
{
    public MixedSampleDescriptor(MixedSample sample)
    {
        Name = sample.Name;
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(MixedSample.Delete)).Disable();
        configuration.Extension("Computed").Register(() => "synthetic value");
        configuration.Extension("Cached").AsStatic().Register(() => "static extension");
        configuration.Extension("Export").Defer(() => "export.bin");
        configuration.Extension("NativeHandle").NotSupported();
    }
}