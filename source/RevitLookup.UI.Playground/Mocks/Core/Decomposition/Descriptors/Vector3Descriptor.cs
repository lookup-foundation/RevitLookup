using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.UI.Playground.Mocks.Core.Decomposition.Descriptors;

public sealed class Vector3Descriptor : Descriptor, IDescriptorResolver
{
    private readonly Vector3 _vector3;

    public Vector3Descriptor(Vector3 vector3)
    {
        _vector3 = vector3;
        Name = $"{vector3.X} {vector3.Y} {vector3.Z}";
    }

    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Vector3.Equals) when parameters[0].ParameterType == typeof(Vector3) => () => Variants.Value(_vector3.Equals(Vector3.Zero), $"Vector-vector comparison"),
            nameof(Vector3.Equals) when parameters.Length == 1 => ResolveObjectEquals,
            _ => null
        };

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        IVariant ResolveObjectEquals()
        {
            return Variants.Values<bool>(3)
                .Add(_vector3.Equals(Vector3.Zero), $"Vector-vector comparison")
                .Add(_vector3.Equals(true), "Vector-Boolean comparison")
                .Add(_vector3.Equals(1), "Vector-Integer comparison")
                .Consume();
        }
    }
}