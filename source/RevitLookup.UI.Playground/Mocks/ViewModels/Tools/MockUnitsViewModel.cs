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

using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using RevitLookup.Abstractions.Models.Tools;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.ViewModels.Tools;
#if NETFRAMEWORK
using RevitLookup.UI.Framework.Extensions;
#endif

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Tools;

[UsedImplicitly]
public sealed partial class MockUnitsViewModel(IVisualDecompositionService decompositionService) : ObservableObject, IUnitsViewModel
{
    [ObservableProperty]
    public partial List<UnitInfo> Units { get; set; } = [];

    [ObservableProperty]
    public partial List<UnitInfo> FilteredUnits { get; set; } = [];

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    public void InitializeParameters()
    {
        Units = new Faker<UnitInfo>()
            .RuleFor(info => info.Unit, faker => faker.Lorem.Sentence())
            .RuleFor(info => info.Label, faker => faker.Lorem.Word())
            .RuleFor(info => info.Value, faker => faker.Lorem.Word())
            .Generate(20);
    }

    public void InitializeCategories()
    {
        Units = new Faker<UnitInfo>()
            .RuleFor(info => info.Unit, faker => faker.Lorem.Sentence())
            .RuleFor(info => info.Label, faker => faker.Lorem.Word())
            .RuleFor(info => info.Value, faker => faker.Lorem.Word())
            .Generate(200);
    }

    public void InitializeForgeSchema()
    {
        Units = new Faker<UnitInfo>()
            .RuleFor(info => info.Unit, faker => faker.Lorem.Sentence())
            .RuleFor(info => info.Label, faker => faker.Lorem.Word())
            .RuleFor(info => info.Value, faker => faker.Lorem.Word())
            .RuleFor(info => info.Class, faker => faker.Lorem.Sentence())
            .Generate(2000);
    }

    public async Task DecomposeAsync(UnitInfo unitInfo)
    {
        await decompositionService.VisualizeDecompositionAsync(unitInfo.Value);
    }

    async partial void OnSearchTextChanged(string value)
    {
        try
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                FilteredUnits = Units;
                return;
            }

            FilteredUnits = await Task.Run(() =>
            {
                var formattedText = value.Trim();
                var searchResults = new List<UnitInfo>();
                foreach (var family in Units)
                {
                    if (family.Label.Contains(formattedText, StringComparison.OrdinalIgnoreCase) ||
                        family.Unit.Contains(formattedText, StringComparison.OrdinalIgnoreCase))
                    {
                        searchResults.Add(family);
                    }
                }

                return searchResults;
            });
        }
        catch
        {
            // ignored
        }
    }

    partial void OnUnitsChanged(List<UnitInfo> value)
    {
        FilteredUnits = value;
    }
}