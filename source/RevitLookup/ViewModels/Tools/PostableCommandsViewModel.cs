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

using Autodesk.Revit.UI;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Models.Tools;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Tools;

namespace RevitLookup.ViewModels.Tools;

[UsedImplicitly]
public sealed partial class PostableCommandsViewModel(ILogger<PostableCommandsViewModel> logger, INotificationService notificationService) : ObservableObject, IPostableCommandsViewModel
{
    [ObservableProperty]
    public partial List<PostableCommandInfo> Commands { get; private set; } = [];

    [ObservableProperty]
    public partial List<PostableCommandInfo> FilteredCommands { get; private set; } = [];

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    public void Initialize()
    {
        var postableCommands = Enum.GetValues<PostableCommand>();
        var postableCommandNames = Enum.GetNames<PostableCommand>();
        Commands = postableCommands
            .Select((command, i) => new PostableCommandInfo
            {
                Name = postableCommandNames[i],
                Value = command
            })
            .OrderBy(static info => info.Name)
            .ToList();
    }

    public void Execute(PostableCommandInfo commandInfo)
    {
        try
        {
            var postableCommand = (PostableCommand) commandInfo.Value;
            var commandId = RevitCommandId.LookupPostableCommandId(postableCommand);
            RevitContext.UiApplication.PostCommand(commandId);
        }
        catch (Exception exception)
        {
            const string message = "Unavailable execute postable command";

            LogExecutePostableCommandFailed(logger, exception);
            notificationService.ShowError(message, exception);
        }
    }

    public bool CanExecute(PostableCommandInfo commandInfo)
    {
        var postableCommand = (PostableCommand) commandInfo.Value;
        var commandId = RevitCommandId.LookupPostableCommandId(postableCommand);
        return RevitContext.UiApplication.CanPostCommand(commandId);
    }

    async partial void OnSearchTextChanged(string value)
    {
        try
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                FilteredCommands = Commands;
                return;
            }

            FilteredCommands = await Task.Run(() =>
            {
                var formattedText = value.Trim();
                return Commands
                    .Where(info => info.Name.Contains(formattedText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            });
        }
        catch
        {
            // ignored
        }
    }

    partial void OnCommandsChanged(List<PostableCommandInfo> value)
    {
        FilteredCommands = value;
    }

    [LoggerMessage(LogLevel.Error, "Unavailable execute postable command")]
    private static partial void LogExecutePostableCommandFailed(ILogger<PostableCommandsViewModel> logger, Exception exception);
}
