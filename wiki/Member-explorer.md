The main feature of RevitLookup is Revit objects analysis, exploring element properties, methods, and events.
This is similar visiting the Revit API documentation, but with evaluated values for specific objects.
All values are evaluated in real-time, which accelerates debugging, plugin development, and family modeling.

RevitLookup provides a comprehensive set of objects to explore,
including root objects such as **Application**, **Document**, **UIApplication**,
as well as specialized objects like **Services**, **Updaters**, and **Performance Issues**.

You can find all these objects on the RevitLookup main page, where each section provides different sets for analysis:

![image](https://github.com/user-attachments/assets/880f1636-efe9-441a-b67c-5da3f3855209)

When navigating to an object, you'll see a complete analysis of properties and methods, evaluated at runtime.
The left side displays the tree of analyzed objects, while the right side shows the members (properties, methods, events) of the selected object and their evaluated values:

![image](https://github.com/user-attachments/assets/516e648a-18fe-43cc-98d5-393eb57adb01)

The table is vertically divided into groups, each providing information about members for a specific type, taking inheritance into account.

For example, the `Id` row in the `Element` group corresponds to the `Id` property of the `Autodesk.Revit.DB.Element` type.

## Visual Representation

To better understand the API, RevitLookup provides visual representations for certain types. For example, properties representing colors have color indicators directly in the UI:

- **Color**. A color value.<br/>
    ![image](https://github.com/user-attachments/assets/139dfc1a-d735-4156-8a1e-deec97c2b04b)
- **Awaiting**. A deferred member that has not been evaluated yet.
- **No return value**. A `void` method that has been successfully evaluated.
- **Disabled**. A member whose evaluation is permanently disabled.
- **Unsupported**. A member the engine cannot evaluate (the overload is not supported, for example).
- **Exception**. A member that threw, marked with an error icon and the message instead of a full red row.
- **Null reference**. A null reference value.
- **Empty string**. A string with zero length.<br/>
    ![image](https://github.com/user-attachments/assets/10a8a69f-8a20-4847-846d-25fae14d9da1)

## Evaluation Variants

Some methods require dynamic input parameters, and RevitLookup can evaluate them for multiple values.
These cases are marked with the `Variants<T>` type and represent a list of possible values, which will strictly depend on what is passed to the method. This feature is particularly useful when exploring methods that can accept different parameter types or when analyzing method overloads. Consider this when using these methods in RevitAPI.

![image](https://github.com/user-attachments/assets/e90c01b3-6a00-40da-9e0d-adc44ea27ffd)

## Deferred Members

RevitLookup automatically evaluates only members it considers safe and fast — those from the `System` and `Autodesk.Revit` namespaces. Slow members, members with side effects on the model, and methods that return `void` are not run automatically when an object opens. They are marked as **Awaiting** in the value column, which keeps opening an object fast and avoids executing model-changing calls without your consent.

To evaluate a deferred member, use the row context menu or a keyboard shortcut:

- **Evaluate** (`F8`) runs the member and fills in its value, computation time, and allocated memory.<br/>
  ![image](https://github.com/user-attachments/assets/43afb300-12fb-400e-bf88-2c2eccbe4a08)
- **Evaluate with transaction** (`Alt + F8`) runs the member inside a Revit transaction, which some members require in order to run. Any changes the member makes are committed to the document.<br/>
  ![image](https://github.com/user-attachments/assets/88d5b411-50f5-4ddc-be29-56eb2c0bb6d2)

Methods that return `void` can be invoked this way as well, and show **No return value** once they have been evaluated.

## Navigation

### Exploring Nested Objects

You may notice that some members are highlighted in color, indicating they contain complex objects or collections of objects.
Click on these rows to open a new window for further analysis of these objects.

![image](https://github.com/user-attachments/assets/ebb50de2-7d59-4a06-a6cd-2860a86b3c4c)

### Multi-Window Analysis

Sometimes you may want to open an object or group of objects in a separate window for convenient viewing. Hold `Ctrl` and click on the object or group of objects.

![image](https://github.com/user-attachments/assets/8fcc645d-e755-46df-8e61-21524e45f256)

Most object members are not available for navigation, as there's no need to analyze primitive types.
However, some may be unsupported classes, or you might want to explore the contents of a class.
Hold `Ctrl` and click on a member to analyze it in a new tab.

![image](https://github.com/user-attachments/assets/61330433-8d25-4ad0-85fb-9b2efe176511)

### Documentation

The context menu includes commands to search for help on specific members or navigate to Microsoft documentation pages for system .NET types.

![image](https://github.com/user-attachments/assets/dab53931-e16f-41b3-a40e-3582f199a4c8)

## Member Display Configuration

Value analysis customization is quite flexible. You can configure what you want to see in the context menu.
Don't want to see events?
Hide them.
Want all API methods?
Enable Unsupported.

### Root Members

By default, RevitLookup hides root members, such as **HashCode** and **Type**. These members are inherited from the base .NET `System.Object` class. You can display them using the following option:

![image](https://github.com/user-attachments/assets/5f033fd2-e294-42ab-ba40-b5d051ae02e9)

### Extension Methods

The RevitAPI includes a large set of Utils classes that don't belong to the object itself but work with it.
RevitLookup allows you to display methods from Utils and other classes directly in the object.
For example, the util method `DocumentValidation.CanDeleteElement` in RevitLookup is available as an extension method `CanDeleteElement` for the `Element` class.

![image](https://github.com/user-attachments/assets/20db8693-a619-4c3b-89a5-62d277377c23)

List of available extensions: https://github.com/lookup-foundation/RevitLookup/wiki/extensions

### Unsupported Members

RevitLookup provides evaluation of properties, void methods, and a large number of parameterized methods.
However, most Revit API methods require dynamic values from the user and strictly depend on the context of use. For example, to calculate a distance, two points are needed, and RevitLookup doesn't know which points to pass to the method.
In such cases, the method is marked as Unsupported and hidden from the results.
To display all available members in the API, enable `Unsupported` in the context menu.

![image](https://github.com/user-attachments/assets/3a30b628-e340-486f-af58-2880402f95be)

### Private Members and Fields

Sometimes Revit API doesn't provide needed properties or methods, or they might be hidden.
Enable the corresponding option to display restricted knowledge.
To call private members in code, you must use reflection only, which is a risky approach and not recommended for use.

![image](https://github.com/user-attachments/assets/e7267c44-469e-4f2e-adc7-58401cec3133)

### Metrics

In addition to evaluated values, you can display performance metrics such as time and memory spent on evaluation.
Only managed memory is analyzed, ignoring C++ allocations.
This can be useful for avoiding frequent calls to slow methods or caching values.
Note that some of these are cached by Revit, so for more accurate results, press `Refresh` multiple times.

![image](https://github.com/user-attachments/assets/430079eb-6905-46de-979f-fa60676d33ee)
