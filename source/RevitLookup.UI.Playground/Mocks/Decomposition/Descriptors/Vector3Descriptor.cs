using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.UI.Playground.Mocks.Decomposition.Descriptors;

/// <summary>
///     Represents a descriptor for a <see cref="System.Numerics.Vector3" /> that resolves its overloaded <see cref="Vector3.Equals(object)" /> members to concrete comparison variants.
/// </summary>
public sealed class Vector3Descriptor : Descriptor, IDescriptorConfigurator
{
    private readonly Vector3 _vector3;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Vector3Descriptor" /> class.
    /// </summary>
    /// <param name="vector3">The vector to describe.</param>
    public Vector3Descriptor(Vector3 vector3)
    {
        _vector3 = vector3;
        Name = $"{vector3.X} {vector3.Y} {vector3.Z}";
    }

    /// <inheritdoc />
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
