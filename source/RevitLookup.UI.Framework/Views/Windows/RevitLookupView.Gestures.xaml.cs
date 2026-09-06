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

using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace RevitLookup.UI.Framework.Views.Windows;

public sealed partial class RevitLookupView
{
    private void AddShortcuts()
    {
        AddCloseShortcut();
        AddCloseAllShortcut();
    }

    private void AddCloseShortcut()
    {
        var command = new RelayCommand(Close);
        InputBindings.Add(new KeyBinding(command, new KeyGesture(Key.Escape)));
    }

    private void AddCloseAllShortcut()
    {
        var command = new RelayCommand(() =>
        {
            for (var i = _intercomService.OpenedWindows.Count - 1; i >= 0; i--)
            {
                var window = _intercomService.OpenedWindows[i];
                window.Close();
            }
        });

        InputBindings.Add(new KeyBinding(command, new KeyGesture(Key.Escape, ModifierKeys.Shift)));
    }
}
