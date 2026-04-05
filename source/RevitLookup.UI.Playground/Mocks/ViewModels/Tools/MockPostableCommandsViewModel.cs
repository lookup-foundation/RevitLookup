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
using RevitLookup.Abstractions.Models.Tools;
using RevitLookup.Abstractions.ViewModels.Tools;

namespace RevitLookup.UI.Playground.Mocks.ViewModels.Tools;

public sealed partial class MockPostableCommandsViewModel : ObservableObject, IPostableCommandsViewModel
{
    [ObservableProperty]
    public partial List<PostableCommandInfo> Commands { get; private set; } = [];

    [ObservableProperty]
    public partial List<PostableCommandInfo> FilteredCommands { get; private set; } = [];

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    public void Initialize()
    {
        Commands = new Faker<PostableCommandInfo>()
            .RuleFor(info => info.Name, faker => faker.Commerce.ProductAdjective() + " " + faker.Commerce.ProductName())
            .RuleFor(info => info.Value, faker => faker.Random.Int())
            .Generate(100)
            .OrderBy(info => info.Name)
            .ToList();
    }

    public void Execute(PostableCommandInfo commandInfo)
    {
        // No-op in playground
    }

    public bool CanExecute(PostableCommandInfo commandInfo)
    {
        return true;
    }

    async partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            FilteredCommands = Commands;
            return;
        }

        FilteredCommands = await Task.Run(() =>
        {
            var formattedText = value.Trim();
            return Commands
                .Where(info => info.Name.Contains(formattedText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        });
    }

    partial void OnCommandsChanged(List<PostableCommandInfo> value)
    {
        FilteredCommands = value;
    }
}
