using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.UI.Playground.Mocks.Core.Decomposition.Descriptors;

public sealed class Vector3Descriptor : Descriptor, IDescriptorConfigurator
{
    private readonly Vector3 _vector3;

    public Vector3Descriptor(Vector3 vector3)
    {
        _vector3 = vector3;
        Name = $"{vector3.X} {vector3.Y} {vector3.Z}";
    }

    public void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(Vector3.Equals))
            .When(parameters => parameters[0].ParameterType == typeof(Vector3))
            .Resolve(() => Variants.Value(_vector3.Equals(Vector3.Zero), "Vector-vector comparison"));
        configuration.Member(nameof(Vector3.Equals))
            .When(parameters => parameters[0].ParameterType == typeof(object))
            .Resolve(ResolveObjectEquals);
        return;

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        IVariant ResolveObjectEquals()
        {
            return Variants.Values<bool>(3)
                .Add(_vector3.Equals(Vector3.Zero), "Vector-vector comparison")
                .Add(_vector3.Equals(true), "Vector-Boolean comparison")
                .Add(_vector3.Equals(1), "Vector-Integer comparison")
                .Consume();
        }
    }
}