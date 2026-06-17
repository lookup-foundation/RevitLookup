using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using RevitLookup.UI.Playground.Mocks.Models;

namespace RevitLookup.UI.Playground.Mocks.Core.Decomposition.Descriptors;

public sealed class DisabledSampleDescriptor : Descriptor, IDescriptorConfigurator
{
    public DisabledSampleDescriptor(DisabledSample sample)
    {
        Name = sample.FileName;
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(DisabledSample.Delete)).Disable();
        configuration.Member(nameof(DisabledSample.Overwrite)).Disable();
    }
}