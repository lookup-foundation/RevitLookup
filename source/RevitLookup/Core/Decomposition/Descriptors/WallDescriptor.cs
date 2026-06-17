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

public sealed class WallDescriptor(Wall wall) : ElementDescriptor(wall)
{
    public override void Configure(IMemberConfigurator configuration)
    {
#if REVIT2022_OR_GREATER
        configuration.Member(nameof(Wall.IsWallCrossSectionValid)).Resolve(() => ResolveEnum<WallCrossSection, bool>(wall.IsWallCrossSectionValid));
#endif

        configuration.Extension("IsJoinAllowedAtEnd").Map(nameof(WallUtils.IsWallJoinAllowedAtEnd)).Register(ResolveIsWallJoinAllowedAtEnd);
        configuration.Extension("AllowJoinAtEnd").Map(nameof(WallUtils.AllowWallJoinAtEnd)).Defer(RegisterAllowJoinAtEnd);
        configuration.Extension("DisallowJoinAtEnd").Map(nameof(WallUtils.DisallowWallJoinAtEnd)).Defer(RegisterDisallowJoinAtEnd);
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

        void RegisterAllowJoinAtEnd()
        {
            wall.AllowJoinAtEnd(0);
            wall.AllowJoinAtEnd(1);
        }

        void RegisterDisallowJoinAtEnd()
        {
            wall.DisallowJoinAtEnd(0);
            wall.DisallowJoinAtEnd(1);
        }
    }
}