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

using System.Reflection;
using System.Text;
using Nice3point.TUnit.Revit;
using Nice3point.TUnit.Revit.Executors;
using TUnit.Core.Executors;

namespace RevitLookup.Tests.Unit;

public sealed class UtilsMethodsTests : RevitApiTest
{
    [Test]
    [TestExecutor<RevitThreadExecutor>]
    public async Task Report_RevitAPI_StaticMethods()
    {
        var outputBuilder = new StringBuilder();
        var assembly = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "RevitAPI");
        
        var types = assembly.GetTypes()
            .Where(type => type is {IsPublic: true, IsClass: true})
            .OrderBy(type => type.Name);

        foreach (var type in types)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(method => !method.IsSpecialName)
                .Where(method => !method.Name.Contains("Create", StringComparison.OrdinalIgnoreCase))
                .Where(method => !method.Name.Contains("Save", StringComparison.OrdinalIgnoreCase))
                .Where(method => !method.Name.Contains("Register", StringComparison.OrdinalIgnoreCase))
                .Where(method => !method.Name.Contains("Relinquish", StringComparison.OrdinalIgnoreCase))
                .Where(method => !method.Name.Contains("Checkout", StringComparison.OrdinalIgnoreCase))
                .Where(method => method.ReturnType != typeof(void))
                .OrderBy(method => method.Name)
                .ToList();

            if (methods.Count == 0) continue;

            foreach (var method in methods)
            {
                var parameters = string.Join(", ", method.GetParameters().Select(parameter => parameter.ParameterType.Name));
                outputBuilder
                    .Append("- ")
                    .Append(type.Name)
                    .Append('.')
                    .Append(method.Name)
                    .Append('(')
                    .Append(parameters)
                    .Append(')')
                    .AppendLine();
            }
        }
        
        var reportPath = $"RevitAPI-StaticMethods-{Application.VersionNumber}.txt";
        await File.WriteAllTextAsync(reportPath, outputBuilder.ToString());

        TestContext.Current!.Output.AttachArtifact(
            reportPath,
            displayName: "Application Logs",
            description: "Logs captured during test execution"
        );
    }
}