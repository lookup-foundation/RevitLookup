// Copyright 2003-2024 by Autodesk, Inc.
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// 
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using Nice3point.Revit.Extensions.Runtime;
using RevitLookup.ViewModels.ObservableObjects;

namespace RevitLookup.Core.Modules.Configuration;

[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
public sealed class RevitConfigurator
{
    private const string BackupSuffix = "_RevitLookupBackup_";
    private const int DefaultBufferSize = 4096; // From System.IO.File source code

    private const char CommentChar = ';';
    private const string SessionOptionsCategory = "[Jrn.SessionOptions]";
    private const string RevitAttributeRecord = " Rvt.Attr.";

    private readonly Encoding _encoding;
    private readonly SemaphoreSlim _asyncLock = new(1, 1);
    private readonly string _userIniPath = Context.Application.CurrentUsersDataFolderPath.AppendPath("Revit.ini");

    private readonly string _defaultIniPath = Environment
        .GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
        .AppendPath("Autodesk")
        .AppendPath($"RVT {Context.Application.VersionNumber}")
        .AppendPath("UserDataCache")
        .AppendPath("Revit.ini");

    private bool _backupDone;

    public RevitConfigurator()
    {
#if NETCOREAPP
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        _encoding = Encoding.GetEncoding(1251);
    }

    public async Task<List<ObservableRevitSettingsEntry>> ReadAsync()
    {
        return await Task.Run(() =>
        {
            var journalConfigurations = ParseJournalSource();
            var userConfigurations = ParseIniFile(false);
            var defaultConfigurations = ParseIniFile(true);

            return MergeSources(journalConfigurations, userConfigurations, defaultConfigurations);
        });
    }

    private List<ObservableRevitSettingsEntry> ParseJournalSource()
    {
        var currentJournal = Context.Application.RecordingJournalFilename;
        var journalsPath = Directory.GetParent(currentJournal)!;
        var journals = Directory.EnumerateFiles(journalsPath.FullName, "journal*txt").Reverse();

        foreach (var journal in journals)
        {
            if (journal == currentJournal) continue;

            var lines = File.ReadLines(journal, _encoding);
            foreach (var sessionOptions in lines.Reverse())
            {
                if (sessionOptions.Contains(SessionOptionsCategory))
                {
                    var startIndex = sessionOptions.IndexOf(SessionOptionsCategory, StringComparison.Ordinal) + SessionOptionsCategory.Length;
                    var optionsPart = sessionOptions[startIndex..];

                    var parts = optionsPart.Split([RevitAttributeRecord], StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => line.Trim())
                        .ToArray();

                    var entries = new List<ObservableRevitSettingsEntry>();
                    foreach (var part in parts)
                    {
                        var keyValue = part.Split([':'], 2);
                        var keyParts = keyValue[0].Split('.');

                        var section = keyParts[0];
                        var entry = keyParts[1];
                        var value = keyValue[1].Trim();

                        entries.Add(new ObservableRevitSettingsEntry
                        {
                            Category = section,
                            Property = entry,
                            Value = value
                        });
                    }

                    return entries;
                }
            }
        }

        return [];
    }

    private List<ObservableRevitSettingsEntry> ParseIniFile(bool useDefault)
    {
        var path = useDefault ? _defaultIniPath : _userIniPath;
        if (!File.Exists(path)) return [];

        var entries = new List<ObservableRevitSettingsEntry>();
        var lines = File.ReadLines(path, Encoding.Unicode);
        var currentCategory = string.Empty;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                currentCategory = trimmedLine.Trim('[', ']');
                continue;
            }

            var isActive = !trimmedLine.StartsWith(CommentChar.ToString());
            if (!isActive)
            {
                trimmedLine = trimmedLine.TrimStart(CommentChar).Trim();
            }

            var keyValue = trimmedLine.Split(['='], 2);
            if (keyValue.Length != 2) continue;

            var property = keyValue[0].Trim();
            var value = keyValue[1].Trim();

            var entry = new ObservableRevitSettingsEntry
            {
                Category = currentCategory,
                Property = property,
                Value = value
            };

            if (useDefault)
            {
                entry.DefaultValue = value;
            }
            else
            {
                entry.IsActive = isActive;
                entry.UserDefined = true;
            }

            entries.Add(entry);
        }

        return entries;
    }

    private List<ObservableRevitSettingsEntry> MergeSources(
        List<ObservableRevitSettingsEntry> journalEntries,
        List<ObservableRevitSettingsEntry> userEntries,
        List<ObservableRevitSettingsEntry> defaultEntries)
    {
        foreach (var userEntry in userEntries)
        {
            var existingEntry = journalEntries.FirstOrDefault(entry => entry.Category == userEntry.Category && entry.Property == userEntry.Property);
            if (existingEntry != null)
            {
                existingEntry.Value = userEntry.Value;
                existingEntry.IsActive = userEntry.IsActive;
                existingEntry.UserDefined = userEntry.UserDefined;
            }
            else
            {
                journalEntries.Add(userEntry);
            }
        }

        foreach (var defaultEntry in defaultEntries)
        {
            var existingEntry = journalEntries.FirstOrDefault(e => e.Category == defaultEntry.Category && e.Property == defaultEntry.Property);
            if (existingEntry != null)
            {
                existingEntry.DefaultValue = defaultEntry.DefaultValue;
            }
            else
            {
                journalEntries.Add(defaultEntry);
            }
        }

        return journalEntries;
    }


    public async Task WriteAsync(List<ObservableRevitSettingsEntry> entries)
    {
        var lines = new List<string>();

        var sortedEntries = entries
            .Where(entry => entry.UserDefined)
            .OrderBy(entry => entry.Category)
            .ThenBy(entry => entry.Property)
            .GroupBy(entry => entry.Category)
            .ToArray();

        foreach (var entryGroup in sortedEntries)
        {
            lines.Add($"[{entryGroup.Key}]");
            foreach (var entry in entryGroup)
            {
                var lineBuilder = new StringBuilder();
                
                if (!entry.IsActive) lineBuilder.Append(CommentChar);
                lineBuilder.Append(entry.Property);
                lineBuilder.Append("=");
                lineBuilder.Append(entry.Value);
                
                lines.Add(lineBuilder.ToString());
            }
        }

        try
        {
            await _asyncLock.WaitAsync();

            var filePath = Path.GetDirectoryName(_userIniPath)!;
            var fileName = Path.GetFileNameWithoutExtension(_userIniPath);
            if (!_backupDone && File.Exists(_userIniPath))
            {
                var backupFileName = $"{fileName}{BackupSuffix}{DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)}.ini";
                File.Copy(_userIniPath!, Path.Combine(filePath, backupFileName));
                _backupDone = true;
            }

            using var stream = new FileStream(_userIniPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, DefaultBufferSize, FileOptions.Asynchronous);
            using var writer = new StreamWriter(stream, Encoding.Unicode);
            foreach (var line in lines)
            {
                await writer.WriteLineAsync(line);
            }

            stream.SetLength(stream.Position);
            await writer.FlushAsync();
        }
        finally
        {
            _asyncLock.Release();
        }
    }
}