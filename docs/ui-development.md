# UI Development

The application UI is built on WPF and shared between the two environments.
Views live in the UI framework project and display in both the production add-in and the Playground.
The display of each data type is driven by data templates that the app selects at runtime, so a new type gains a tailored presentation without a change to the view.

## The ViewModel Pattern

A view binds to a contract, and each environment supplies its own implementation, so the production host shows real Revit data and the Playground shows mock data.

To add a window or page:

1. Define the ViewModel contract in the abstractions project.
2. Implement the contract twice. The production implementation reads real data from the Revit API in the add-in project. The Playground implementation returns mock data.
3. Skip manual registration. Views and ViewModels register automatically through Scrutor assembly scanning. See [Architecture](./architecture.md).

## Customize a Type's Display

The object tree picks a data template per item through a `DataTemplateSelector`.
To present a specific type its own way, define a template and add a rule that selects it.

1. Define a `DataTemplate` in a XAML resource for the type:

```xml
<DataTemplate
    x:Key="DefaultSummaryTreeItemTemplate"
    DataType="{x:Type decomposition:ObservableDecomposedObject}">
    <ui:TextBlock
        FontTypography="Caption"
        Text="{Binding .,
            Converter={x:Static converters:DescriptorFormattingConverters.ObjectDisplayText},
            Mode=OneTime}" />
</DataTemplate>
```

2. Add a rule to the template selector that maps the value to the template:

```csharp
public sealed class TreeViewItemTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item is null) return null;

        var presenter = (FrameworkElement) container;
        var decomposedObject = (ObservableDecomposedObject) item;
        var templateName = decomposedObject.RawValue switch
        {
            Color => "SummaryMediaColorItemTemplate",
            _ => "DefaultSummaryTreeItemTemplate"
        };

        return (DataTemplate) presenter.FindResource(templateName);
    }
}
```

For custom visualization of further types, follow the same pattern and register the template with the matching selector.

## The Playground

The Playground hosts the shared UI as a standalone WPF application with mock ViewModels, so UI work needs no running Revit.
Use it to develop and check layouts, themes, templates, and interactions, then verify the production behavior under Revit when the change touches the Revit API.
See [Testing](./testing.md) for the Playground as a UI test environment.
