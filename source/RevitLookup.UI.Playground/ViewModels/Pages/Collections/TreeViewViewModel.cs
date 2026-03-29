using Bogus;
using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Person = RevitLookup.UI.Playground.Models.Person;

namespace RevitLookup.UI.Playground.ViewModels.Pages.Collections;

[UsedImplicitly]
public sealed class TreeViewViewModel : ObservableObject
{
    public List<Person> Persons { get; } = GenerateHierarchicalPersons();

    private static List<Person> GenerateHierarchicalPersons()
    {
        var employeeFaker = new Faker<Person>()
            .RuleFor(person => person.FirstName, faker => faker.Person.FirstName)
            .RuleFor(person => person.LastName, faker => faker.Person.LastName)
            .RuleFor(person => person.Company, faker => faker.Company.CompanyName("{{name.lastName}}"));

        var departmentFaker = new Faker<Person>()
            .RuleFor(person => person.FirstName, faker => faker.Person.FirstName)
            .RuleFor(person => person.LastName, faker => faker.Person.LastName)
            .RuleFor(person => person.Company, faker => faker.Company.CompanyName("{{name.lastName}}"))
            .RuleFor(person => person.Children, faker => employeeFaker.Generate(faker.Random.Int(3, 7)));

        var companyFaker = new Faker<Person>()
            .RuleFor(person => person.FirstName, faker => faker.Person.FirstName)
            .RuleFor(person => person.LastName, faker => faker.Person.LastName)
            .RuleFor(person => person.Company, faker => faker.Company.CompanyName("{{name.lastName}}"))
            .RuleFor(person => person.Children, faker => departmentFaker.Generate(faker.Random.Int(3, 5)));

        return companyFaker.Generate(5);
    }
}