# UI Development & Styling

The application UI is data-template based, with templates customizable for different data types. Templates are located in `source/RevitLookup/Styles/ComponentStyles/` directory.

## UI Architecture

RevitLookup uses a shared UI approach where Views are defined in `RevitLookup.UI.Framework` and used in both Revit and Playground environments.

When adding a new window or page:

1. Define a ViewModel interface in `RevitLookup.Abstractions`.
2. Implement the interface twice:
    * **Revit implementation:** in `RevitLookup.ViewModels`, providing real data from the Revit API.
    * **Playground implementation:** in `RevitLookup.UI.Playground.Mocks.ViewModels`, providing mock or sample data.
3. Registration in the DI container is not required; views and ViewModels are automatically registered via Scrutor.

## Customizing Type Display

To customize the display of a specific type:

1. Create a DataTemplate in a XAML file within the ComponentStyles directory:

```xml
// source/RevitLookup/Styles/ComponentStyles/ObjectsTree/TreeGroupTemplates.xaml
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

2. Add a selector rule in the `TemplateSelector` class:

```csharp
// source/RevitLookup/Styles/ComponentStyles/ObjectsTree/TreeViewItemTemplateSelector.cs
public sealed class TreeViewItemTemplateSelector : DataTemplateSelector
{
    /// <summary>
    ///     Tree view row style selector
    /// </summary>
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

For custom visualization of specific data types, create specialized templates following the pattern above and register them in the appropriate style selectors.
