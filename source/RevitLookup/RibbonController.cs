// Copyright 2003-2023 by Autodesk, Inc.
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

using Autodesk.Revit.UI;
using Autodesk.Windows;
using Nice3point.Revit.Extensions.UI;
using RevitLookup.Commands;
using RevitLookup.Core;
using RevitLookup.Services.Contracts;
using RevitLookup.Utils;

namespace RevitLookup;

public static class RibbonController
{
    private const string PanelName = "Revit Lookup";

    public static void CreatePanel(UIControlledApplication application, ISettingsService settingsService)
    {
        var addinPanel = application.CreatePanel("Revit Lookup");
        var pullButton = addinPanel.AddPullDownButton("RevitLookupButton", "RevitLookup");
        pullButton.SetImage("/RevitLookup;component/Resources/Images/RibbonIcon16.png");
        pullButton.SetLargeImage("/RevitLookup;component/Resources/Images/RibbonIcon32.png");

        pullButton.AddPushButton<DashboardCommand>("Dashboard");
        ResolveSelectionButton(settingsService, pullButton);
        pullButton.AddPushButton<SnoopViewCommand>("Snoop Active view");
        pullButton.AddPushButton<SnoopDocumentCommand>("Snoop Document");
        pullButton.AddPushButton<SnoopDatabaseCommand>("Snoop Database");
        pullButton.AddPushButton<SnoopFaceCommand>("Snoop Face");
        pullButton.AddPushButton<SnoopEdgeCommand>("Snoop Edge");
        pullButton.AddPushButton<SnoopPointCommand>("Snoop Point");
        pullButton.AddPushButton<SnoopLinkedElementCommand>("Snoop Linked element");
        pullButton.AddPushButton<SearchElementsCommand>("Search Elements");
        pullButton.AddPushButton<EventMonitorCommand>("Event monitor");
    }

    private static void ResolveSelectionButton(ISettingsService settingsService, PulldownButton parentButton)
    {
        if (!settingsService.UseModifyTab)
        {
            parentButton.AddPushButton<SnoopSelectionCommand>("Snoop Selection");
            return;
        }

        var modifyTab = ComponentManager.Ribbon.FindTab("Modify");
        var modifyPanel = modifyTab.CreatePanel(PanelName);

        var button = modifyPanel.AddPushButton<SnoopSelectionCommand>("Snoop\nSelection");
        button.SetImage("/RevitLookup;component/Resources/Images/RibbonIcon16.png");
        button.SetLargeImage("/RevitLookup;component/Resources/Images/RibbonIcon32.png");
    }

    public static void ReloadPanels(ISettingsService settingsService)
    {
        Application.ActionEventHandler.Raise(_ =>
        {
            RibbonUtils.RemovePanel("CustomCtrl_%CustomCtrl_%Add-Ins%Revit Lookup%RevitLookupButton", PanelName);
            RibbonUtils.RemovePanel("CustomCtrl_%Revit Lookup%RevitLookup.Commands.SnoopSelectionCommand", PanelName);

            var controlledApplication = RevitApi.CreateUiControlledApplication();
            CreatePanel(controlledApplication, settingsService);

            RibbonUtils.ReloadShortcuts();
        });
    }
}