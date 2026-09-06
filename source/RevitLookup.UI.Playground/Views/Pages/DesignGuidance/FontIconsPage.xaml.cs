using System.Windows;
using System.Windows.Input;
using RevitLookup.UI.Playground.ViewModels.Pages.DesignGuidance;

namespace RevitLookup.UI.Playground.Views.Pages.DesignGuidance;

/// <summary>
///     Represents a page that demonstrates the font icon glyphs available in the Playground.
/// </summary>
public sealed partial class FontIconsPage
{
    static FontIconsPage()
    {
        CommandManager.RegisterClassCommandBinding(typeof(FontIconsPage), new CommandBinding(ApplicationCommands.Copy, OnCopyContentClicked));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FontIconsPage" /> class.
    /// </summary>
    /// <param name="viewModel">The view model that supplies data for the page.</param>
    public FontIconsPage(FontIconsPageViewModel viewModel)
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
