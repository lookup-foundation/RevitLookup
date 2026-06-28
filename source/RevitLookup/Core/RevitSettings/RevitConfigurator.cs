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

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using Nice3point.Revit.Extensions.Runtime;
using RevitLookup.Abstractions.ObservableModels.Entries;

namespace RevitLookup.Core.RevitSettings;

[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
public sealed class RevitConfigurator
{
    private const string BackupSuffix = "_RevitLookupBackup_";
    private const int DefaultBufferSize = 4096; // System.IO.File source code

    private const char CommentChar = ';';
    private const string SessionOptionsCategory = "[Jrn.SessionOptions]";
    private const string RevitAttributeRecord = " Rvt.Attr.";

    private readonly Encoding _encoding;
    private readonly SemaphoreSlim _asyncLock = new(1, 1);
    private readonly string _userIniPath = RevitApiContext.Application.CurrentUsersDataFolderPath.AppendPath("Revit.ini");

    private readonly string _defaultIniPath = Environment
        .GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
        .AppendPath("Autodesk")
        .AppendPath($"RVT {RevitApiContext.Application.VersionNumber}")
        .AppendPath("UserDataCache")
        .AppendPath("Revit.ini");

    private bool _backupDone;

    public RevitConfigurator()
    {
#if NET
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        _encoding = Encoding.GetEncoding(1251);
    }

    public async Task<List<ObservableIniEntry>> ReadAsync()
    {
        return await Task.Run(() =>
        {
            var journalConfigurations = ParseJournalSource();
            var userConfigurations = ParseIniFile(false);
            var defaultConfigurations = ParseIniFile(true);

            return MergeSources(journalConfigurations, userConfigurations, defaultConfigurations);
        });
    }

    private List<ObservableIniEntry> ParseJournalSource()
    {
        var currentJournal = RevitApiContext.Application.RecordingJournalFilename;
        var journalsPath = Directory.GetParent(currentJournal)!;
        var journals = Directory.EnumerateFiles(journalsPath.FullName, "journal*txt").Reverse();

        foreach (var journal in journals)
        {
            if (journal == currentJournal) continue;

            var lines = File.ReadLines(journal, _encoding);
            foreach (var sessionOptions in lines.Reverse())
            {
                if (!sessionOptions.Contains(SessionOptionsCategory)) continue;

                var startIndex = sessionOptions.IndexOf(SessionOptionsCategory, StringComparison.Ordinal) + SessionOptionsCategory.Length;
                var optionsPart = sessionOptions[startIndex..];

                var parts = optionsPart.Split([RevitAttributeRecord], StringSplitOptions.RemoveEmptyEntries)
                    .Select(static line => line.Trim())
                    .ToArray();

                var entries = new List<ObservableIniEntry>();
                foreach (var part in parts)
                {
                    var keyValue = part.Split([':'], 2);
                    var keyParts = keyValue[0].Split('.');

                    var section = keyParts[0];
                    var entry = keyParts[1];
                    var value = keyValue[1].Trim();

                    entries.Add(new ObservableIniEntry
                    {
                        Category = section,
                        Property = entry,
                        Value = value
                    });
                }

                return entries;
            }
        }

        return [];
    }

    private List<ObservableIniEntry> ParseIniFile(bool useDefault)
    {
        var path = useDefault ? _defaultIniPath : _userIniPath;
        if (!File.Exists(path)) return [];

        var entries = new List<ObservableIniEntry>();
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

            var entry = new ObservableIniEntry
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

    private List<ObservableIniEntry> MergeSources(
        List<ObservableIniEntry> journalEntries,
        List<ObservableIniEntry> userEntries,
        List<ObservableIniEntry> defaultEntries)
    {
        var index = new Dictionary<(string Category, string Property), ObservableIniEntry>(journalEntries.Count);
        foreach (var entry in journalEntries)
        {
            index.TryAdd((entry.Category, entry.Property), entry);
        }

        foreach (var userEntry in userEntries)
        {
            if (index.TryGetValue((userEntry.Category, userEntry.Property), out var existingEntry))
            {
                existingEntry.Value = userEntry.Value;
                existingEntry.IsActive = userEntry.IsActive;
                existingEntry.UserDefined = userEntry.UserDefined;
            }
            else
            {
                journalEntries.Add(userEntry);
                index[(userEntry.Category, userEntry.Property)] = userEntry;
            }
        }

        foreach (var defaultEntry in defaultEntries)
        {
            if (index.TryGetValue((defaultEntry.Category, defaultEntry.Property), out var existingEntry))
            {
                existingEntry.DefaultValue = defaultEntry.DefaultValue;
            }
            else
            {
                journalEntries.Add(defaultEntry);
                index[(defaultEntry.Category, defaultEntry.Property)] = defaultEntry;
            }
        }

        return journalEntries;
    }


    public async Task WriteAsync(List<ObservableIniEntry> entries)
    {
        var lines = new List<string>();

        var sortedEntries = entries
            .Where(static entry => entry.UserDefined)
            .OrderBy(static entry => entry.Category)
            .ThenBy(static entry => entry.Property)
            .GroupBy(static entry => entry.Category)
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
                var backupFileName = $"{fileName}{BackupSuffix}{DateTime.Now.ToString("yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo)}.ini";
                File.Copy(_userIniPath, Path.Combine(filePath, backupFileName));
                _backupDone = true;
            }

#if NET
            await using var stream = new FileStream(_userIniPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, DefaultBufferSize, FileOptions.Asynchronous);
            await using var writer = new StreamWriter(stream, Encoding.Unicode);
#else
            using var stream = new FileStream(_userIniPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, DefaultBufferSize, FileOptions.Asynchronous);
            using var writer = new StreamWriter(stream, Encoding.Unicode);
#endif
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