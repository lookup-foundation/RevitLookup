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
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Tools;
using RevitLookup.Abstractions.ViewModels.Tools;
#if NETFRAMEWORK
using RevitLookup.UI.Framework.Menus;
#endif

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Tools;

/// <summary>
///     Represents a Playground mock of <see cref="IUnitsViewModel" /> that fabricates parameter, category, and Forge schema entries with <c>Bogus</c>.
/// </summary>
/// <param name="decompositionService">The service that visualizes the decomposition of the selected unit value.</param>
[UsedImplicitly]
public sealed partial class MockUnitsViewModel(IVisualDecompositionService decompositionService) : ObservableObject, IUnitsViewModel
{
    /// <inheritdoc />
    [ObservableProperty]
    public partial List<UnitInfo> Units { get; set; } = [];

    /// <inheritdoc />
    [ObservableProperty]
    public partial List<UnitInfo> FilteredUnits { get; set; } = [];

    /// <inheritdoc />
    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    /// <inheritdoc />
    public void InitializeParameters()
    {
        Units = new Faker<UnitInfo>()
            .RuleFor(info => info.Unit, faker => faker.Lorem.Sentence())
            .RuleFor(info => info.Label, faker => faker.Lorem.Word())
            .RuleFor(info => info.Value, faker => faker.Lorem.Word())
            .Generate(20);
    }

    /// <inheritdoc />
    public void InitializeCategories()
    {
        Units = new Faker<UnitInfo>()
            .RuleFor(info => info.Unit, faker => faker.Lorem.Sentence())
            .RuleFor(info => info.Label, faker => faker.Lorem.Word())
            .RuleFor(info => info.Value, faker => faker.Lorem.Word())
            .Generate(200);
    }

    /// <inheritdoc />
    public void InitializeForgeSchema()
    {
        Units = new Faker<UnitInfo>()
            .RuleFor(info => info.Unit, faker => faker.Lorem.Sentence())
            .RuleFor(info => info.Label, faker => faker.Lorem.Word())
            .RuleFor(info => info.Value, faker => faker.Lorem.Word())
            .RuleFor(info => info.Class, faker => faker.Lorem.Sentence())
            .Generate(2000);
    }

    /// <inheritdoc />
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
