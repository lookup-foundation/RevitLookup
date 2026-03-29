using System.Windows.Controls;
using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Person = RevitLookup.UI.Playground.Models.Person;

namespace RevitLookup.UI.Playground.ViewModels.Pages.Collections;

[UsedImplicitly]
public sealed partial class ListBoxViewModel : ObservableObject
{
    [ObservableProperty]
    public partial int SelectionModeIndex { get; set; }

    [ObservableProperty]
    public partial SelectionMode SelectionMode { get; private set; } = SelectionMode.Single;

    [ObservableProperty]
    public partial List<Person> Persons { get; set; } = new Faker<Person>()
        .RuleFor(person => person.FirstName, faker => faker.Person.FirstName)
        .RuleFor(person => person.LastName, faker => faker.Person.LastName)
        .RuleFor(person => person.Company, faker => faker.Company.CompanyName("{{name.lastName}}"))
        .Generate(50);

    partial void OnSelectionModeIndexChanged(int value)
    {
        SelectionMode = value switch
        {
            1 => SelectionMode.Multiple,
            2 => SelectionMode.Extended,
            _ => SelectionMode.Single
        };
    }
}