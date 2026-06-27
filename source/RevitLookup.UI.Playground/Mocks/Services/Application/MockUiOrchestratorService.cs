using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.ObservableModels.Decomposition;
using RevitLookup.Abstractions.Services.Application;
using RevitLookup.Abstractions.Services.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.UI.Framework.Views.Windows;
using Wpf.Ui;

namespace RevitLookup.UI.Playground.Mocks.Services.Application;

public sealed partial class MockUiOrchestratorService : IUiOrchestratorService, IHistoryOrchestrator
{
    private IServiceProvider? _parentProvider;
    private readonly List<Task> _activeTasks = [];
    private readonly IServiceScope _scope;
    private readonly IDecompositionService _decompositionService;
    private readonly IVisualDecompositionService _visualDecompositionService;
    private readonly INavigationService _navigationService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<MockUiOrchestratorService> _logger;
    private readonly Window _host;

    public MockUiOrchestratorService(IServiceScopeFactory scopeFactory)
    {
        _scope = scopeFactory.CreateScope();

        _host = _scope.ServiceProvider.GetRequiredService<RevitLookupView>();
        _decompositionService = _scope.ServiceProvider.GetRequiredService<IDecompositionService>();
        _visualDecompositionService = _scope.ServiceProvider.GetRequiredService<IVisualDecompositionService>();
        _navigationService = _scope.ServiceProvider.GetRequiredService<INavigationService>();
        _notificationService = _scope.ServiceProvider.GetRequiredService<INotificationService>();
        _logger = _scope.ServiceProvider.GetRequiredService<ILogger<MockUiOrchestratorService>>();

        _host.Closed += (_, _) => _scope.Dispose();
    }

    public INavigationOrchestrator Decompose(KnownDecompositionObject decompositionObject)
    {
        PushTask();
        return this;

        async void PushTask()
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
    }

    public INavigationOrchestrator Decompose(object? obj)
    {
        PushTask();
        return this;

        async void PushTask()
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
    }

    public INavigationOrchestrator Decompose(IEnumerable objects)
    {
        PushTask();
        return this;

        async void PushTask()
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
    }

    public INavigationOrchestrator Decompose(ObservableDecomposedObject decomposedObject)
    {
        PushTask();
        return this;

        async void PushTask()
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
    }

    public INavigationOrchestrator Decompose(List<ObservableDecomposedObject> decomposedObjects)
    {
        PushTask();
        return this;

        async void PushTask()
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
    }

    public IHistoryOrchestrator AddParent(IServiceProvider parentProvider)
    {
        PushTask();
        return this;

        async void PushTask()
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
    }

    public IDecompositionOrchestrator AddStackHistory(ObservableDecomposedObject item)
    {
        PushTask();
        return this;

        async void PushTask()
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
    }

    public IInteractionOrchestrator Show<T>() where T : Page
    {
        PushTask();
        return this;

        async void PushTask()
        {
            try
            {
                await Task.WhenAll(_activeTasks);
            }
            catch (Exception exception)
            {
                LogStartupError(_logger, exception);
                _notificationService.ShowError("Lookup engine error", exception);
            }
            finally
            {
                ShowHost(false);
                _navigationService.Navigate(typeof(T));
            }
        }
    }

    public void RunService<T>(Action<T> handler) where T : class
    {
        PushTask();
        return;

        async void PushTask()
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
            _host.Show();
            _host.Focus();
        }
    }

    [LoggerMessage(LogLevel.Error, "RevitLookup new instance startup error")]
    private static partial void LogStartupError(ILogger<MockUiOrchestratorService> logger, Exception exception);
}