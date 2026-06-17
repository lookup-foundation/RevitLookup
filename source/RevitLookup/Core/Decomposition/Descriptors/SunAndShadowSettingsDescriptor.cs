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

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed class SunAndShadowSettingsDescriptor(SunAndShadowSettings settings) : ElementDescriptor(settings)
{
    public override void Configure(IMemberConfigurator configuration)
    {
        configuration.Member(nameof(SunAndShadowSettings.GetActiveSunAndShadowSettings)).Resolve(() => SunAndShadowSettings.GetActiveSunAndShadowSettings(settings.Document));
        configuration.Member(nameof(SunAndShadowSettings.GetSunrise)).Resolve(() => settings.GetSunrise(DateTime.Today));
        configuration.Member(nameof(SunAndShadowSettings.GetSunset)).Resolve(() => settings.GetSunset(DateTime.Today));
        configuration.Member(nameof(SunAndShadowSettings.IsTimeIntervalValid)).Resolve(() => ResolveEnum<SunStudyTimeInterval, bool>(settings.IsTimeIntervalValid));
        configuration.Member(nameof(SunAndShadowSettings.IsAfterStartDateAndTime)).Resolve(() => settings.IsAfterStartDateAndTime(DateTime.Today));
        configuration.Member(nameof(SunAndShadowSettings.IsBeforeEndDateAndTime)).Resolve(() => settings.IsBeforeEndDateAndTime(DateTime.Today));
    }
}