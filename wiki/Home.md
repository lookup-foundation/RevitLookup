RevitLookup is a free and open-source plugin for Autodesk Revit that provides a powerful way to explore and interact with the Revit API without using a debugger. It helps you
easily view Revit element parameters, such as their identifiers, types, parameters, and properties, as well as access low-level model information, such as geometry, graphics, and
material data.

RevitLookup is a very useful tool for all Revit users, especially those developing their own plug-ins. It helps you access model information quickly and easily, which reduces the
time it takes to analyze and manage your project.

## RevitLookup Features

### Member Explorer

[<img align="left" src="https://github.com/user-attachments/assets/516e648a-18fe-43cc-98d5-393eb57adb01" width="350" />](https://github.com/lookup-foundation/RevitLookup/wiki/Member-Explorer)

[Member Explorer](https://github.com/lookup-foundation/RevitLookup/wiki/Member-Explorer) explores properties, methods, events of any Revit object in Runtime.
Values are evaluated on the fly, speeding up debugging, plugin development, or family modeling.
RevitLookup provides a pretty large set of objects to explore,
including the root objects Application, Document, UIApplication, as well as niche objects like Services, Updaters, and Performance Issues.
<br/>
<br/>
<br/>

### Event Monitor

<img align="left" src="https://github.com/user-attachments/assets/bf3f7615-40bb-4bc8-b0e0-a075642de805" width="350" />

Event Monitor collects and analyses most of the events that occur in the Revit API.
These can be both application-wide and project-wide events.
Monitoring only works when the tool is open, for performance reasons.
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>

### Visualization

[<img align="left" src="https://github.com/user-attachments/assets/c979a541-8c73-4a90-95ed-451f13e4a1ea" width="350" />](https://github.com/lookup-foundation/RevitLookup/wiki/Visualization)

[Visualization](https://github.com/lookup-foundation/RevitLookup/wiki/Visualization) provides a visual representation of the geometry in the Revit API, how it is arranged in space and what part of the complete object it is.
RevitLookup supports all commonly used geometry primitives, and supports multi-visualization, just open multiple instances of RevitLookup side by side.
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>

### Revit Units

<img align="left" src="https://github.com/user-attachments/assets/dd23b07f-720f-401c-85cc-f7edf25fe641" width="350" />

RevitLookup provides an analysis of all commonly used Revit enumerations and units.
Built-in parameters, Built-in categories, Forge units are all available out of the box,
and in the context menu you can also go to [Member Explorer](https://github.com/lookup-foundation/RevitLookup/wiki/Member-Explorer)
to explore the properties and methods of these units.
<br/>
<br/>
<br/>
<br/>

### Search Elements

[<img align="left" src="https://github.com/user-attachments/assets/0008d7fc-8f81-4b10-9145-0139f54dad46" width="350" />](https://github.com/lookup-foundation/RevitLookup/wiki/Search-Elements)

[Search](https://github.com/lookup-foundation/RevitLookup/wiki/Search-Elements) helps users to quickly find doors, walls, windows and other elements in the project.
Search supports complex queries, search for multiple elements at once, search by both ID and Name, your element will always be found
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>

### Revit.ini File editor

[<img align="left" src="https://github.com/user-attachments/assets/169a076a-767c-4456-86ca-399ce9cccf99" width="350" />](https://github.com/lookup-foundation/RevitLookup/wiki/Revit.ini-File-Editor)

[Revit.ini File editor](https://github.com/lookup-foundation/RevitLookup/wiki/Revit.ini-File-Editor) provides a simple way to manage Revit settings without a manual editing Revit.ini file.
Revit.ini is quite complicated to configure, and not all Revit configuration can be found in the official documentation. Editor will provide all available properties that are possible.
<br/>
<br/>
<br/>
<br/>

### Modules

<img align="left" src="https://github.com/user-attachments/assets/dde98dde-6c04-4dd0-8010-e4d13eeae018" width="350" /> 

Modules view inspect the dynamic link libraries (DLLs) and executables that Revit uses.
In this view, you'll find information such as module names, versions, application domains, paths to the module.
Especially useful for debugging plugins, and analyzing the dependencies they use.