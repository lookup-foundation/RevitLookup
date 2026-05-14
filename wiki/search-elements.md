RevitLookup includes a powerful search function that allows users to quickly find specific elements in a project.

The search supports searching by `Name`, `Id`, `UniqueId`, `IfcGUID` and `Type IfcGUID` parameters.

![image](https://user-images.githubusercontent.com/20504884/225869706-7d5e2e4a-1f03-416e-9ad6-a96184b07836.png)

Searching for items by `IfcGUID` is more convenient because it is accessible from the Revit properties panel and is always at your fingertips

![image](https://user-images.githubusercontent.com/20504884/225117536-daf8a0ad-ccfd-4632-b6f4-07654cdf6970.png)

Searching by `Type IfcGUID` finds all instances of a certain type. This is convenient for researching and searching all instances in RevitLookup. Search by `Id` does not have this capability:

![image](https://user-images.githubusercontent.com/20504884/225117066-a8d80f50-8bb9-4ccf-84b4-ace798e52858.png)

You can search for multiple values at once. If you use different delimiters between values, separate them with a new line. For one separator, such as a space, you can write the query on one line, even with different types

![image](https://user-images.githubusercontent.com/20504884/226112177-4d78990e-9319-4018-94fe-c9fc00260699.png)

List of available delimiters in priority order:

* Tab
* Semicolon
* Comma
* Space

An example using different delimiters. The elements `Door, 800x2060` and `Door, 1100x2150` will be found, because the semicolon prevails over the comma:

![image](https://user-images.githubusercontent.com/20504884/226112579-74fda64b-fb44-4c58-97e9-8cf06acc41a4.png)