using System.Windows;
using System.Windows.Input;
using RevitLookup.UI.Playground.ViewModels.Pages.DesignGuidance;

namespace RevitLookup.UI.Playground.Views.Pages.DesignGuidance;

/// <summary>
///     Represents a page that demonstrates the symbol icon glyphs available in the Playground.
/// </summary>
public sealed partial class SymbolIconsPage
{
    static SymbolIconsPage()
    {
        CommandManager.RegisterClassCommandBinding(typeof(SymbolIconsPage), new CommandBinding(ApplicationCommands.Copy, OnCopyContentClicked));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SymbolIconsPage" /> class.
    /// </summary>
    /// <param name="viewModel">The view model that supplies data for the page.</param>
    public SymbolIconsPage(SymbolIconsPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private static void OnCopyContentClicked(object sender, RoutedEventArgs args)
    {
        var routedArgs = (ExecutedRoutedEventArgs)args;
        var parameter = routedArgs.Parameter.ToString();

        if (!string.IsNullOrEmpty(parameter))
        {
            try
            {
                Clipboard.SetText(parameter);
            }
            catch
            {
                // ignored
            }
        }
    }
}
