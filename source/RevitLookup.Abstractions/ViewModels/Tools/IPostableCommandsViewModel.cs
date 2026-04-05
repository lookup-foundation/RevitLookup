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

using RevitLookup.Abstractions.Models.Tools;

namespace RevitLookup.Abstractions.ViewModels.Tools;

/// <summary>
///     Represents the data for the Postable Commands view.
/// </summary>
public interface IPostableCommandsViewModel
{
    /// <summary>
    ///     The list of all commands.
    /// </summary>
    List<PostableCommandInfo> Commands { get; }

    /// <summary>
    ///     The list of filtered commands.
    /// </summary>
    List<PostableCommandInfo> FilteredCommands { get; }

    /// <summary>
    ///     The search query for filtering commands.
    /// </summary>
    string SearchText { get; set; }

    /// <summary>
    ///     Initialize commands for representation.
    /// </summary>
    void Initialize();

    /// <summary>
    ///     Execute command.
    /// </summary>
    void Execute(PostableCommandInfo commandInfo);

    /// <summary>
    ///     Check if command can be executed.
    /// </summary>
    bool CanExecute(PostableCommandInfo commandInfo);
}
