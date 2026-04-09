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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Nice3point.Revit.Extensions.UI;
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
        if (_dispatcher.CheckAccess())
        {
            _lookupService = new LookupServiceImpl(scopeFactory);
        }
        else
        {
            _dispatcher.Invoke(() => _lookupService = new LookupServiceImpl(scopeFactory));
        }
    }
    
    public ILookupServiceDependsStage Snoop(SnoopableType snoopableType)
    {
        if (_dispatcher.CheckAccess())
        {
            _lookupService.Snoop(snoopableType);
        }
        else
        {
            _dispatcher.Invoke(() => _lookupService.Snoop(snoopableType));
        }
        
        return this;
    }
    
    public ILookupServiceDependsStage Snoop(SnoopableObject snoopableObject)
    {
        if (_dispatcher.CheckAccess())
        {
            _lookupService.Snoop(snoopableObject);
        }
        else
        {
            _dispatcher.Invoke(() => _lookupService.Snoop(snoopableObject));
        }
        
        return this;
    }
    
    public ILookupServiceDependsStage Snoop(IList<SnoopableObject> snoopableObjects)
    {
        if (_dispatcher.CheckAccess())
        {
            _lookupService.Snoop(snoopableObjects);
        }
        else
        {
            _dispatcher.Invoke(() => _lookupService.Snoop(snoopableObjects));
        }
        
        return this;
    }
    
    public ILookupServiceShowStage DependsOn(IServiceProvider provider)
    {
        if (_dispatcher.CheckAccess())
        {
            _lookupService.DependsOn(provider);
        }
        else
        {
            _dispatcher.Invoke(() => _lookupService.DependsOn(provider));
        }
        
        return this;
    }
    
    public ILookupServiceExecuteStage Show<T>() where T : Page
    {
        if (_dispatcher.CheckAccess())
        {
            _lookupService.Show<T>();
        }
        else
        {
            _dispatcher.Invoke(() => _lookupService.Show<T>());
        }
        
        return this;
    }
    
    public void Execute<T>(Action<T> handler) where T : class
    {
        if (_dispatcher.CheckAccess())
        {
            _lookupService.Execute(handler);
        }
        else
        {
            _dispatcher.Invoke(() => _lookupService.Execute(handler));
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
        
        // We must yield
        // Sometimes the Dispatcher is unavailable for current thread
        Thread.Sleep(1);
        
        _dispatcher = dispatcher;
    }
    
    private sealed class LookupServiceImpl
    {
        private Window _owner;
        private Task _activeTask;
        private readonly IServiceScope _scope;
        private readonly ISnoopVisualService _visualService;
        private readonly INavigationService _navigationService;
        private readonly Window _window;
        
        public LookupServiceImpl(IServiceScopeFactory scopeFactory)
        {
            _scope = scopeFactory.CreateScope();
            
            _window = (Window) _scope.ServiceProvider.GetRequiredService<IWindow>();
            _visualService = _scope.ServiceProvider.GetRequiredService<ISnoopVisualService>();
            _navigationService = _scope.ServiceProvider.GetRequiredService<INavigationService>();
            
            _window.Closed += (_, _) => _scope.Dispose();
        }
        
        public void Snoop(SnoopableType snoopableType)
        {
            if (_activeTask is null || _activeTask.IsCompleted)
            {
                _activeTask = _visualService!.SnoopAsync(snoopableType);
            }
            else
            {
                _activeTask = _activeTask.ContinueWith(_ => _visualService!.SnoopAsync(snoopableType), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        
        public void Snoop(SnoopableObject snoopableObject)
        {
            _visualService.Snoop(snoopableObject);
        }
        
        public void Snoop(IList<SnoopableObject> snoopableObjects)
        {
            _visualService.Snoop(snoopableObjects);
        }
        
        public void DependsOn(IServiceProvider provider)
        {
            _owner = (Window) provider.GetService<IWindow>();
        }
        
        public void Show<T>() where T : Page
        {
            if (_activeTask is null || _activeTask.IsCompleted)
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
            if (_activeTask is null || _activeTask.IsCompleted)
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
            var service = _scope.ServiceProvider.GetRequiredService<T>();
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
            
            _window.Show(Context.UiApplication.MainWindowHandle);
            _navigationService.Navigate(typeof(T));
        }
    }
}