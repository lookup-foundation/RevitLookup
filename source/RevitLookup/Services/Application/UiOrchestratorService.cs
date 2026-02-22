using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Autodesk.Revit.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.ObservableModels.Decomposition;
using RevitLookup.Abstractions.Services.Application;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.UI.Framework.Views.Windows;
using Wpf.Ui;

namespace RevitLookup.Services.Application;

public sealed class UiOrchestratorService : IUiOrchestratorService, IHistoryOrchestrator
{
    private static readonly Dispatcher Dispatcher;
    private UiServiceImpl _uiService = null!; //Late init in the constructor

    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    static UiOrchestratorService()
    {
        using var dispatcherReadyEvent = new ManualResetEventSlim(false);
        var uiThread = new Thread(() =>
        {
            //Create a new Dispatcher
            _ = Dispatcher.CurrentDispatcher;
            dispatcherReadyEvent.Set();

            //Borrow a thread
            Dispatcher.Run();
        });

        uiThread.SetApartmentState(ApartmentState.STA);
        uiThread.IsBackground = true;
        uiThread.Start();

        dispatcherReadyEvent.Wait();
        Dispatcher = Dispatcher.FromThread(uiThread)!;
    }

    public UiOrchestratorService(IServiceScopeFactory scopeFactory)
    {
        if (Dispatcher.CheckAccess())
        {
            _uiService = new UiServiceImpl(scopeFactory);
        }
        else
        {
            Dispatcher.Invoke(() => _uiService = new UiServiceImpl(scopeFactory));
        }
    }

    public INavigationOrchestrator Decompose(KnownDecompositionObject decompositionObject)
    {
        if (Dispatcher.CheckAccess())
        {
            _uiService.Decompose(decompositionObject);
        }
        else
        {
            Dispatcher.Invoke(() => _uiService.Decompose(decompositionObject));
        }

        return this;
    }

    public INavigationOrchestrator Decompose(object? obj)
    {
        if (Dispatcher.CheckAccess())
        {
            _uiService.Decompose(obj);
        }
        else
        {
            Dispatcher.Invoke(() => _uiService.Decompose(obj));
        }

        return this;
    }

    public INavigationOrchestrator Decompose(IEnumerable objects)
    {
        if (Dispatcher.CheckAccess())
        {
            _uiService.Decompose(objects);
        }
        else
        {
            Dispatcher.Invoke(() => _uiService.Decompose(objects));
        }

        return this;
    }

    public INavigationOrchestrator Decompose(ObservableDecomposedObject decomposedObject)
    {
        if (Dispatcher.CheckAccess())
        {
            _uiService.Decompose(decomposedObject);
        }
        else
        {
            Dispatcher.Invoke(() => _uiService.Decompose(decomposedObject));
        }

        return this;
    }

    public INavigationOrchestrator Decompose(List<ObservableDecomposedObject> decomposedObjects)
    {
        if (Dispatcher.CheckAccess())
        {
            _uiService.Decompose(decomposedObjects);
        }
        else
        {
            Dispatcher.Invoke(() => _uiService.Decompose(decomposedObjects));
        }

        return this;
    }

    public IHistoryOrchestrator AddParent(IServiceProvider parentProvider)
    {
        if (Dispatcher.CheckAccess())
        {
            _uiService.AddParent(parentProvider);
        }
        else
        {
            Dispatcher.Invoke(() => _uiService.AddParent(parentProvider));
        }

        return this;
    }

    public IDecompositionOrchestrator AddStackHistory(ObservableDecomposedObject item)
    {
        if (Dispatcher.CheckAccess())
        {
            _uiService.AddStackHistory(item);
        }
        else
        {
            Dispatcher.Invoke(() => _uiService.AddStackHistory(item));
        }

        return this;
    }

    public IInteractionOrchestrator Show<T>() where T : Page
    {
        if (Dispatcher.CheckAccess())
        {
            _uiService.Show<T>();
        }
        else
        {
            Dispatcher.Invoke(() => _uiService.Show<T>());
        }

        return this;
    }

    public void RunService<T>(Action<T> handler) where T : class
    {
        if (Dispatcher.CheckAccess())
        {
            _uiService.RunService(handler);
        }
        else
        {
            Dispatcher.Invoke(() => _uiService.RunService(handler));
        }
    }

    private sealed class UiServiceImpl
    {
        private IServiceProvider? _parentProvider;
        private readonly List<Task> _activeTasks = [];
        private readonly IServiceScope _scope;
        private readonly IDecompositionService _decompositionService;
        private readonly IVisualDecompositionService _visualDecompositionService;
        private readonly INavigationService _navigationService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<UiOrchestratorService> _logger;
        private readonly Window _host;

        public UiServiceImpl(IServiceScopeFactory scopeFactory)
        {
            _scope = scopeFactory.CreateScope();

            _host = _scope.ServiceProvider.GetRequiredService<RevitLookupView>();
            _decompositionService = _scope.ServiceProvider.GetRequiredService<IDecompositionService>();
            _visualDecompositionService = _scope.ServiceProvider.GetRequiredService<IVisualDecompositionService>();
            _navigationService = _scope.ServiceProvider.GetRequiredService<INavigationService>();
            _notificationService = _scope.ServiceProvider.GetRequiredService<INotificationService>();
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<UiOrchestratorService>>();

            _host.Closed += (_, _) => _scope.Dispose();
        }

        public async void Decompose(KnownDecompositionObject decompositionObject)
        {
            try
            {
                await Task.WhenAll(_activeTasks);
            }
            catch
            {
                // ignored
            }
            finally
            {
                _activeTasks.Add(_visualDecompositionService.VisualizeDecompositionAsync(decompositionObject));
            }
        }

        public async void Decompose(object? obj)
        {
            try
            {
                await Task.WhenAll(_activeTasks);
            }
            catch
            {
                // ignored
            }
            finally
            {
                _activeTasks.Add(_visualDecompositionService.VisualizeDecompositionAsync(obj));
            }
        }

        public async void Decompose(IEnumerable objects)
        {
            try
            {
                await Task.WhenAll(_activeTasks);
            }
            catch
            {
                // ignored
            }
            finally
            {
                _activeTasks.Add(_visualDecompositionService.VisualizeDecompositionAsync(objects));
            }
        }

        public async void Decompose(ObservableDecomposedObject decomposedObject)
        {
            try
            {
                await Task.WhenAll(_activeTasks);
            }
            catch
            {
                // ignored
            }
            finally
            {
                _activeTasks.Add(_visualDecompositionService.VisualizeDecompositionAsync(decomposedObject));
            }
        }

        public async void Decompose(List<ObservableDecomposedObject> decomposedObjects)
        {
            try
            {
                await Task.WhenAll(_activeTasks);
            }
            catch
            {
                // ignored
            }
            finally
            {
                _activeTasks.Add(_visualDecompositionService.VisualizeDecompositionAsync(decomposedObjects));
            }
        }

        public async void AddParent(IServiceProvider parentProvider)
        {
            try
            {
                await Task.WhenAll(_activeTasks);
            }
            catch
            {
                // ignored
            }
            finally
            {
                var decompositionService = parentProvider.GetRequiredService<IDecompositionService>();
                _decompositionService.DecompositionStackHistory.AddRange(decompositionService.DecompositionStackHistory);
                _parentProvider = parentProvider;
            }
        }

        public async void AddStackHistory(ObservableDecomposedObject item)
        {
            try
            {
                await Task.WhenAll(_activeTasks);
            }
            catch
            {
                // ignored
            }
            finally
            {
                _decompositionService.DecompositionStackHistory.Add(item);
            }
        }

        public async void Show<T>() where T : Page
        {
            try
            {
                await Task.WhenAll(_activeTasks);
            }
            catch (InvalidObjectException exception)
            {
                _notificationService.ShowError("Invalid object", exception);
            }
            catch (InternalException)
            {
                _notificationService.ShowError(
                    "Invalid object",
                    "A problem in the Revit code. Usually occurs when a managed API object is no longer valid and is unloaded from memory");
            }
            catch (SEHException)
            {
                _notificationService.ShowError(
                    "Revit API internal error",
                    "A problem in the Revit code. Usually occurs when a managed API object is no longer valid and is unloaded from memory");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "RevitLookup new instance startup error");
                _notificationService.ShowError("Lookup engine error", exception);
            }
            finally
            {
                ShowHost(false);
                _navigationService.Navigate(typeof(T));
            }
        }

        public async void RunService<T>(Action<T> handler) where T : class
        {
            try
            {
                await Task.WhenAll(_activeTasks);
            }
            catch
            {
                // ignored
            }
            finally
            {
                var service = _scope.ServiceProvider.GetRequiredService<T>();
                handler.Invoke(service);
            }
        }

        private void ShowHost(bool modal)
        {
            if (_parentProvider is null)
            {
                _host.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                var parentHost = _parentProvider.GetRequiredService<IWindowIntercomService>().GetHost();

                _host.WindowStartupLocation = WindowStartupLocation.Manual;
                _host.Left = parentHost.Left + 47;
                _host.Top = parentHost.Top + 49;
            }

            if (modal)
            {
                _host.ShowDialog();
            }
            else
            {
                _host.Show(RevitContext.UiApplication.MainWindowHandle);
            }
        }
    }
}