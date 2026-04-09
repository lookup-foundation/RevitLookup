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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Nice3point.Revit.Extensions.UI;
using RevitLookup.Core;
using RevitLookup.Core.Objects;
using RevitLookup.Services.Contracts;
using RevitLookup.Services.Enums;
using Wpf.Ui;

namespace RevitLookup.Services;

public sealed class LookupService : ILookupService
{
    private static Dispatcher _dispatcher;
    private LookupServiceImpl _lookupService;

    static LookupService()
    {
        var uiThread = new Thread(Dispatcher.Run);
        uiThread.SetApartmentState(ApartmentState.STA);
        uiThread.Start();

        EnsureThreadStart(uiThread);
    }

    public LookupService(IServiceScopeFactory scopeFactory)
    {
        if (Thread.CurrentThread == _dispatcher.Thread)
        {
            _lookupService = new LookupServiceImpl(scopeFactory);
        }
        else
        {
            _dispatcher.InvokeAsync(() => _lookupService = new LookupServiceImpl(scopeFactory));
        }
    }

    public ILookupServiceDependsStage Snoop(SnoopableType snoopableType)
    {
        if (Thread.CurrentThread == _dispatcher.Thread)
        {
            _lookupService.Snoop(snoopableType);
        }
        else
        {
            _dispatcher.InvokeAsync(() => _lookupService.Snoop(snoopableType));
        }

        return this;
    }

    public ILookupServiceDependsStage Snoop(SnoopableObject snoopableObject)
    {
        if (Thread.CurrentThread == _dispatcher.Thread)
        {
            _lookupService.Snoop(snoopableObject);
        }
        else
        {
            _dispatcher.InvokeAsync(() => _lookupService.Snoop(snoopableObject));
        }

        return this;
    }

    public ILookupServiceDependsStage Snoop(IReadOnlyCollection<SnoopableObject> snoopableObjects)
    {
        if (Thread.CurrentThread == _dispatcher.Thread)
        {
            _lookupService.Snoop(snoopableObjects);
        }
        else
        {
            _dispatcher.InvokeAsync(() => _lookupService.Snoop(snoopableObjects));
        }

        return this;
    }

    public ILookupServiceShowStage DependsOn(IServiceProvider provider)
    {
        if (Thread.CurrentThread == _dispatcher.Thread)
        {
            _lookupService.DependsOn(provider);
        }
        else
        {
            _dispatcher.InvokeAsync(() => _lookupService.DependsOn(provider));
        }

        return this;
    }

    public ILookupServiceExecuteStage Show<T>() where T : Page
    {
        if (Thread.CurrentThread == _dispatcher.Thread)
        {
            _lookupService.Show<T>();
        }
        else
        {
            _dispatcher.InvokeAsync(() => _lookupService.Show<T>());
        }

        return this;
    }

    public void Execute<T>(Action<T> handler) where T : class
    {
        if (Thread.CurrentThread == _dispatcher.Thread)
        {
            _lookupService.Execute(handler);
        }
        else
        {
            _dispatcher.InvokeAsync(() => _lookupService.Execute(handler));
        }
    }

    private static void EnsureThreadStart(Thread thread)
    {
        Dispatcher dispatcher = null;
        SpinWait spinWait = new();
        while (dispatcher is null)
        {
            spinWait.SpinOnce();
            dispatcher = Dispatcher.FromThread(thread);
        }

        _dispatcher = dispatcher;
    }

    private class LookupServiceImpl
    {
        private Window _owner;
        private Task _activeTask;
        private readonly IServiceScope _serviceScope;
        private readonly ISnoopVisualService _visualService;
        private readonly INavigationService _navigationService;
        private readonly Window _window;

        public LookupServiceImpl(IServiceScopeFactory scopeFactory)
        {
            _serviceScope = scopeFactory.CreateScope();

            _window = (Window) _serviceScope.ServiceProvider.GetService<IWindow>();
            _visualService = _serviceScope.ServiceProvider.GetService<ISnoopVisualService>();
            _navigationService = _serviceScope.ServiceProvider.GetService<INavigationService>();

            _window.Closed += (_, _) => _serviceScope.Dispose();
        }

        public void Snoop(SnoopableType snoopableType)
        {
            _activeTask = _visualService!.SnoopAsync(snoopableType);
        }

        public void Snoop(SnoopableObject snoopableObject)
        {
            _visualService.Snoop(snoopableObject);
        }

        public void Snoop(IReadOnlyCollection<SnoopableObject> snoopableObjects)
        {
            _visualService.Snoop(snoopableObjects);
        }

        public void DependsOn(IServiceProvider provider)
        {
            _owner = (Window) provider.GetService<IWindow>();
        }

        public void Show<T>() where T : Page
        {
            if (_activeTask is null)
            {
                ShowPage<T>();
            }
            else
            {
                _activeTask = _activeTask.ContinueWith(_ => ShowPage<T>(), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public void Execute<T>(Action<T> handler) where T : class
        {
            if (_activeTask is null)
            {
                InvokeHandler(handler);
            }
            else
            {
                _activeTask = _activeTask.ContinueWith(_ => InvokeHandler(handler), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void InvokeHandler<T>(Action<T> handler) where T : class
        {
            var service = _serviceScope.ServiceProvider.GetService<T>();
            handler.Invoke(service);
        }

        private void ShowPage<T>() where T : Page
        {
            if (_owner is null)
            {
                _window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                _window.Left = _owner.Left + 47;
                _window.Top = _owner.Top + 49;
            }

            _window.Show(RevitApi.UiApplication.MainWindowHandle);
            _navigationService.Navigate(typeof(T));
        }
    }
}