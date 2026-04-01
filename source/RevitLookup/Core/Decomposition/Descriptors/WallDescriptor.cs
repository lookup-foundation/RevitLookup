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

using System.Reflection;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class WallDescriptor(Wall wall) : ElementDescriptor(wall)
{
    public override Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
#if REVIT2022_OR_GREATER
            nameof(Wall.IsWallCrossSectionValid) => () => VariantsResolver.ResolveEnum<WallCrossSection, bool>(wall.IsWallCrossSectionValid),
#endif
            _ => null
        };
    }

    public override void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define("IsJoinAllowedAtEnd").Map(nameof(WallUtils.IsWallJoinAllowedAtEnd)).Register(ResolveIsWallJoinAllowedAtEnd);
        manager.Define("AllowJoinAtEnd").Map(nameof(WallUtils.AllowWallJoinAtEnd)).AsNotSupported();
        manager.Define("DisallowJoinAtEnd").Map(nameof(WallUtils.DisallowWallJoinAtEnd)).AsNotSupported();
        return;

        IVariant ResolveIsWallJoinAllowedAtEnd()
        {
            var isJoinAllowedAtEnd0 = WallUtils.IsWallJoinAllowedAtEnd(wall, 0);
            var isJoinAllowedAtEnd1 = WallUtils.IsWallJoinAllowedAtEnd(wall, 1);

            return Variants.Values<bool>(2)
                .Add(isJoinAllowedAtEnd0, $"Start: {isJoinAllowedAtEnd0}")
                .Add(isJoinAllowedAtEnd1, $"End: {isJoinAllowedAtEnd1}")
                .Consume();
        }
    }

}