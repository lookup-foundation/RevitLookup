using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using Person = RevitLookup.UI.Playground.Models.Person;

namespace RevitLookup.UI.Playground.ViewModels.Pages.Collections;

[UsedImplicitly]
public sealed partial class DataGridViewModel : ObservableObject
{
    [ObservableProperty]
    public partial List<Person> Persons { get; set; } = new Faker<Person>()
        .RuleFor(person => person.FirstName, faker => faker.Person.FirstName)
        .RuleFor(person => person.LastName, faker => faker.Person.LastName)
        .RuleFor(person => person.Company, faker => faker.PickRandom("RevitLookup", "Autodesk"))
        .Generate(25);
}