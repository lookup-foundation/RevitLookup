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

using RevitLookup.UI.Framework.Processes;

namespace RevitLookup.UI.Framework.Menus;

/// <summary>
///     Provides methods that open help for a search query in the default browser.
/// </summary>
public static class ContextHelpUtils
{
    /// <summary>
    ///     Opens help for the specified query in the default browser.
    /// </summary>
    /// <param name="query">
    ///     The search query. A query starting with "System" is resolved against the .NET API documentation; otherwise, it is searched on DuckDuckGo.
    /// </param>
    public static void ShowHelp(string query)
    {
        string uri;

        if (query.Contains(' '))
        {
            uri = $"https://duckduckgo.com/?q={query}";
        }
        else if (query.StartsWith("System"))
        {
            query = query.Replace('`', '-');
            uri = $"https://docs.microsoft.com/en-us/dotnet/api/{query}";
        }
        else
        {
            uri = $"https://duckduckgo.com/?q={query}";
        }

        ProcessTasks.StartShell(uri);
    }

    /// <summary>
    ///     Opens help for a query qualified by a member or parameter name in the default browser.
    /// </summary>
    /// <param name="query">The base search query, such as a type name.</param>
    /// <param name="parameter">The member or parameter name to append to <paramref name="query" />.</param>
    public static void ShowHelp(string query, string parameter)
    {
        if (query.StartsWith("System"))
        {
            ShowHelp($"{query}.{parameter}");
        }
        else
        {
            ShowHelp($"{query} {parameter}");
        }
    }
}
