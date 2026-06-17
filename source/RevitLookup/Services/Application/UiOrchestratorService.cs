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

public sealed class UiOrchestratorService(IServiceScopeFactory scopeFactory) : IUiOrchestratorService, IHistoryOrchestrator
{
    private static readonly Dispatcher Dispatcher;
    private UiSession? _session;

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

    public INavigationOrchestrator Decompose(KnownDecompositionObject decompositionObject)
    {
        var session = EnsureSession();
        InvokeOnDispatcher(() => session.Decompose(decompositionObject));
        return this;
    }

    public INavigationOrchestrator Decompose(object? obj)
    {
        var session = EnsureSession();
        InvokeOnDispatcher(() => session.Decompose(obj));
        return this;
    }

    public INavigationOrchestrator Decompose(IEnumerable objects)
    {
        var session = EnsureSession();
        InvokeOnDispatcher(() => session.Decompose(objects));
        return this;
    }

    public INavigationOrchestrator Decompose(ObservableDecomposedObject decomposedObject)
    {
        var session = EnsureSession();
        InvokeOnDispatcher(() => session.Decompose(decomposedObject));
        return this;
    }

    public INavigationOrchestrator Decompose(List<ObservableDecomposedObject> decomposedObjects)
    {
        var session = EnsureSession();
        InvokeOnDispatcher(() => session.Decompose(decomposedObjects));
        return this;
    }

    public IHistoryOrchestrator AddParent(IServiceProvider parentProvider)
    {
        var session = EnsureSession();
        InvokeOnDispatcher(() => session.AddParent(parentProvider));
        return this;
    }

    public IDecompositionOrchestrator AddStackHistory(ObservableDecomposedObject item)
    {
        var session = EnsureSession();
        InvokeOnDispatcher(() => session.AddStackHistory(item));
        return this;
    }

    public IInteractionOrchestrator Show<T>() where T : Page
    {
        var session = EnsureSession();
        InvokeOnDispatcher(() => session.Show<T>());
        return this;
    }

    public void RunService<T>(Action<T> handler) where T : class
    {
        if (_session is {IsAlive: true})
        {
            var session = _session;
            InvokeOnDispatcher(() => session.RunService(handler));
        }
        else
        {
            var scopeFactoryInstance = scopeFactory;
            InvokeOnDispatcher(() =>
            {
                using var scope = scopeFactoryInstance.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<T>();
                handler.Invoke(service);
            });
        }
    }

    private UiSession EnsureSession()
    {
        if (_session is {IsAlive: true})
        {
            return _session;
        }

        _session = Dispatcher.CheckAccess() ? new UiSession(scopeFactory) : Dispatcher.Invoke(() => new UiSession(scopeFactory));
        return _session;
    }

    private static void InvokeOnDispatcher(Action action)
    {
        if (Dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            Dispatcher.Invoke(action);
        }
    }

    private sealed class UiSession
    {
        private IServiceProvider? _parentProvider;

        private readonly List<Task> _activeTasks = [];
        private readonly IServiceScope _scope;
        private readonly IVisualDecompositionService _visualDecompositionService;
        private readonly ILogger<UiOrchestratorService> _logger;
        private readonly Window _host;

        public bool IsAlive { get; private set; } = true;

        public UiSession(IServiceScopeFactory scopeFactory)
        {
            _scope = scopeFactory.CreateScope();
            _visualDecompositionService = _scope.ServiceProvider.GetRequiredService<IVisualDecompositionService>();
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<UiOrchestratorService>>();
            _host = _scope.ServiceProvider.GetRequiredService<RevitLookupView>();

            _host.Closed += OnHostClosed;
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
                var parentDecompositionService = parentProvider.GetRequiredService<IDecompositionService>();
                var decompositionService = _scope.ServiceProvider.GetRequiredService<IDecompositionService>();
                decompositionService.DecompositionStackHistory.AddRange(parentDecompositionService.DecompositionStackHistory);
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
                var decompositionService = _scope.ServiceProvider.GetRequiredService<IDecompositionService>();
                decompositionService.DecompositionStackHistory.Add(item);
            }
        }

        public async void Show<T>() where T : Page
        {
            try
            {
                await NotifyErrorsAsync();
                ShowHost();

                _scope.ServiceProvider.GetRequiredService<INavigationService>().Navigate(typeof(T));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "RevitLookup new instance startup error");
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

        private void ShowHost()
        {
            ConfigureWindowLocation();
            _host.Show(RevitContext.UiApplication.MainWindowHandle);
        }

        private void ConfigureWindowLocation()
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
        }

        private async Task NotifyErrorsAsync()
        {
            var notificationService = _scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                await Task.WhenAll(_activeTasks);
            }
            catch (InvalidObjectException exception)
            {
                notificationService.ShowError("Invalid object", exception);
            }
            catch (InternalException)
            {
                notificationService.ShowError(
                    "Invalid object",
                    "A problem in the Revit code. Usually occurs when a managed API object is no longer valid and is unloaded from memory");
            }
            catch (SEHException)
            {
                notificationService.ShowError(
                    "Revit API internal error",
                    "A problem in the Revit code. Usually occurs when a managed API object is no longer valid and is unloaded from memory");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Lookup engine error");
                notificationService.ShowError("Lookup engine error", exception);
            }
        }

        private void OnHostClosed(object? sender, EventArgs e)
        {
            IsAlive = false;
            _scope.Dispose();
        }
    }
}