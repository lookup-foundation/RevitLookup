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

using System.Diagnostics;
using System.Reflection;
using Autodesk.Revit.UI;
using Microsoft.Extensions.Logging;
using RevitLookup.Services.Application;

namespace RevitLookup.Services.Decomposition;

public sealed class EventsMonitoringService(ILogger<EventsMonitoringService> logger)
{
    private Action<object, string>? _callback;
    private readonly Dictionary<EventInfo, Delegate> _handlersMap = new(16);

    private readonly Assembly[] _assemblies = AppDomain.CurrentDomain
        .GetAssemblies()
        .Where(assembly =>
        {
            var name = assembly.GetName().Name;
            return name is "RevitAPI" or "RevitAPIUI";
        })
        .Take(2)
        .ToArray();

    private readonly List<string> _denyList =
    [
        nameof(UIApplication.Idling),
        nameof(Autodesk.Revit.ApplicationServices.Application.ProgressChanged)
    ];

    public void RegisterEventInvocationCallback(Action<object, string> callback)
    {
        _callback = callback;
        EventHandlers.ActionEventHandler.Raise(Subscribe);
    }

    public void Unregister()
    {
        EventHandlers.ActionEventHandler.Raise(Unsubscribe);
    }

    private void Subscribe(UIApplication uiApplication)
    {
        if (_handlersMap.Count > 0) return;

        foreach (var dll in _assemblies)
        foreach (var type in dll.GetTypes())
        foreach (var eventInfo in type.GetEvents())
        {
            if (_denyList.Contains(eventInfo.Name)) continue;

            var targets = FindValidTargets(eventInfo.ReflectedType);
            if (targets.Length == 0)
            {
                logger.LogDebug("Missing target: {EventType}.{EventName}", eventInfo.ReflectedType, eventInfo.Name);
                break;
            }

            var methodInfo = GetType().GetMethod(nameof(OnHandlingEvent), BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)!;
            var eventHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType!, this, methodInfo);

            foreach (var target in targets)
            {
                eventInfo.AddEventHandler(target, eventHandler);
            }

            _handlersMap.Add(eventInfo, eventHandler);
            logger.LogDebug("Observing: {EventType}.{EventName}", eventInfo.ReflectedType, eventInfo.Name);
        }
    }

    private void Unsubscribe(UIApplication uiApplication)
    {
        foreach (var eventInfo in _handlersMap)
        {
            var targets = FindValidTargets(eventInfo.Key.ReflectedType);
            foreach (var target in targets)
            {
                eventInfo.Key.RemoveEventHandler(target, eventInfo.Value);
            }
        }

        _handlersMap.Clear();
    }

    private static object[] FindValidTargets(Type? targetType)
    {
        if (targetType == typeof(Document)) return RevitApiContext.Application.Documents.Cast<object>().ToArray();
        if (targetType == typeof(Autodesk.Revit.ApplicationServices.Application)) return [RevitApiContext.Application];
        if (targetType == typeof(UIApplication)) return [RevitContext.UiApplication];

        return [];
    }

    public void OnHandlingEvent(object sender, EventArgs args)
    {
        var stackTrace = new StackTrace();
        var stackFrames = stackTrace.GetFrames()!;
        var eventType = stackFrames[1].GetMethod()!.Name;
        var eventName = eventType.Replace(nameof(EventHandler), "");

        _callback?.Invoke(args, eventName);
    }
}