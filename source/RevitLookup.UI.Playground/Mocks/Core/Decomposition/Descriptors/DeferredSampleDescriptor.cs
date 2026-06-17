using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using RevitLookup.UI.Playground.Mocks.Models;

namespace RevitLookup.UI.Playground.Mocks.Core.Decomposition.Descriptors;

public sealed class DeferredSampleDescriptor : Descriptor, IDescriptorConfigurator
{
    public DeferredSampleDescriptor(DeferredSample sample)
    {
        Name = sample.Title;
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(DeferredSample.CalculateTotals)).Defer();
        configuration.Member(nameof(DeferredSample.BuildChart)).Defer();
        configuration.Member(nameof(DeferredSample.ExportToPdf)).Defer();
    }
}