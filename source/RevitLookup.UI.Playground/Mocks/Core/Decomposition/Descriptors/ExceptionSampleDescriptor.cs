using LookupEngine.Abstractions.Decomposition;
using RevitLookup.UI.Playground.Mocks.Models;

namespace RevitLookup.UI.Playground.Mocks.Core.Decomposition.Descriptors;

public sealed class ExceptionSampleDescriptor : Descriptor
{
    public ExceptionSampleDescriptor(ExceptionSample sample)
    {
        Name = sample.Endpoint;
    }
}