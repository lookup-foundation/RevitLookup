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
using Autodesk.Revit.UI;
using Microsoft.Extensions.Logging;
using Nice3point.Revit.Toolkit.External;

namespace RevitLookup.Services.Decomposition;

public sealed partial class EventsMonitoringService(ILogger<EventsMonitoringService> logger)
{
    private Action<object, string>? _eventInvoked;
    private readonly Dictionary<EventInfo, Delegate> _handlersMap = new(16);
    private static readonly MethodInfo HandlerMethod = typeof(EventHandlerWrapper).GetMethod(nameof(EventHandlerWrapper.OnEvent))!;

    private readonly Assembly[] _assemblies = AppDomain.CurrentDomain
        .GetAssemblies()
        .Where(assembly =>
        {
            var name = assembly.GetName().Name;
            return name is "RevitAPI" or "RevitAPIUI";
        })
        .Take(2)
        .ToArray();

    private readonly HashSet<string> _denyList =
    [
        nameof(UIApplication.Idling),
        nameof(Autodesk.Revit.ApplicationServices.Application.ProgressChanged)
    ];

    public event Action<object, string> EventInvoked
    {
        add
        {
            _eventInvoked += value;
            SubscribeEvent.Raise();
        }
        remove
        {
            _eventInvoked -= value;
            if (_eventInvoked is null)
            {
                UnsubscribeEvent.Raise();
            }
        }
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private void Subscribe()
    {
        if (_handlersMap.Count > 0) return;

        foreach (var dll in _assemblies)
        foreach (var type in dll.GetTypes().Where(type => type is {IsEnum: false, IsValueType: false, IsInterface: false}))
        foreach (var eventInfo in type.GetEvents())
        {
            if (_denyList.Contains(eventInfo.Name)) continue;

            var targets = FindValidTargets(eventInfo.ReflectedType);
            if (targets.Length == 0)
            {
                logger.LogDebug("Missing target: {EventType}.{EventName}", eventInfo.ReflectedType, eventInfo.Name);
                break;
            }

            var wrapper = new EventHandlerWrapper(eventInfo.Name, this);
            var eventHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType!, wrapper, HandlerMethod);

            foreach (var target in targets)
            {
                eventInfo.AddEventHandler(target, eventHandler);
            }

            _handlersMap.Add(eventInfo, eventHandler);
            logger.LogDebug("Observing: {EventType}.{EventName}", eventInfo.ReflectedType, eventInfo.Name);
        }
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private void Unsubscribe()
    {
        foreach (var (eventInfo, handler) in _handlersMap)
        {
            var targets = FindValidTargets(eventInfo.ReflectedType);
            foreach (var target in targets)
            {
                eventInfo.RemoveEventHandler(target, handler);
            }
        }

        _handlersMap.Clear();
    }

    private static object[] FindValidTargets(Type? targetType) => targetType switch
    {
        _ when targetType == typeof(Document)
            => RevitApiContext.Application.Documents.Cast<object>().ToArray(),
        _ when targetType == typeof(Autodesk.Revit.ApplicationServices.Application)
            => [RevitApiContext.Application],
        _ when targetType == typeof(UIApplication)
            => [RevitContext.UiApplication],
        _ => []
    };

    private sealed class EventHandlerWrapper(string eventName, EventsMonitoringService service)
    {
        [UsedImplicitly(Reason = "Reflection delegate subscription")]
        public void OnEvent(object sender, EventArgs args)
        {
            service._eventInvoked?.Invoke(args, eventName);
        }
    }
}