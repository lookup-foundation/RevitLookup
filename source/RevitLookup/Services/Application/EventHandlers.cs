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

using System.Collections;
using Nice3point.Revit.Toolkit.External;
using RevitLookup.Abstractions.ObservableModels.Decomposition;

namespace RevitLookup.Services.Application;

public static class EventHandlers
{
    public static ActionEventHandler ActionEventHandler
    {
        get => field ?? throw new InvalidOperationException("The Handler was never set.");
        private set;
    }

    public static AsyncEventHandler AsyncEventHandler
    {
        get => field ?? throw new InvalidOperationException("The Handler was never set.");
        private set;
    }

    public static AsyncEventHandler<ObservableDecomposedObject> AsyncObjectHandler
    {
        get => field ?? throw new InvalidOperationException("The Handler was never set.");
        private set;
    }

    public static AsyncEventHandler<List<ObservableDecomposedObject>> AsyncObjectsHandler
    {
        get => field ?? throw new InvalidOperationException("The Handler was never set.");
        private set;
    }

    public static AsyncEventHandler<List<ObservableDecomposedMember>> AsyncMembersHandler
    {
        get => field ?? throw new InvalidOperationException("The Handler was never set.");
        private set;
    }

    public static AsyncEventHandler<IEnumerable> AsyncCollectionHandler
    {
        get => field ?? throw new InvalidOperationException("The Handler was never set.");
        private set;
    }

    public static void RegisterHandlers()
    {
        ActionEventHandler = new ActionEventHandler();
        AsyncEventHandler = new AsyncEventHandler();
        AsyncObjectHandler = new AsyncEventHandler<ObservableDecomposedObject>();
        AsyncObjectsHandler = new AsyncEventHandler<List<ObservableDecomposedObject>>();
        AsyncMembersHandler = new AsyncEventHandler<List<ObservableDecomposedMember>>();
        AsyncCollectionHandler = new AsyncEventHandler<IEnumerable>();
    }
}