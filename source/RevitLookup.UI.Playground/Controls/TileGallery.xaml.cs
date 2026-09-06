using System.Windows;

namespace RevitLookup.UI.Playground.Controls;

/// <summary>
///     Represents a horizontally scrollable gallery of tiles with scroll buttons.
/// </summary>
public sealed partial class TileGallery
{
    public TileGallery()
    {
        InitializeComponent();
    }

    private void OnScrollButtonClicked(object sender, RoutedEventArgs e)
    {
        var newOffSet = RootScrollViewer.HorizontalOffset - 210;
        RootScrollViewer.ScrollToHorizontalOffset(newOffSet);
        UpdateScrollButtonsVisibility(newOffSet);
    }

    private void OnScrollForwardButtonClicked(object sender, RoutedEventArgs e)
    {
        var newOffSet = RootScrollViewer.HorizontalOffset + 210;
        RootScrollViewer.ScrollToHorizontalOffset(newOffSet);
        UpdateScrollButtonsVisibility(newOffSet);
    }

    private void UpdateScrollButtonsVisibility()
    {
        var offset = RootScrollViewer.HorizontalOffset;
        UpdateScrollButtonsVisibility(offset);
    }

    private void UpdateScrollButtonsVisibility(double newOffset)
    {
        ScrollBackButton.Visibility = Visibility.Visible;
        ScrollForwardButton.Visibility = Visibility.Visible;

        if (RootScrollViewer.ActualWidth < TilesPanel.ActualWidth)
        {
            if (newOffset == 0)
            {
                ScrollBackButton.Visibility = Visibility.Collapsed;
            }
            else if (newOffset >= RootScrollViewer.ScrollableWidth)
            {
                ScrollForwardButton.Visibility = Visibility.Collapsed;
            }
        }
        else
        {
            ScrollBackButton.Visibility = Visibility.Collapsed;
            ScrollForwardButton.Visibility = Visibility.Collapsed;
        }
    }

    private void OnRootScrollSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateScrollButtonsVisibility();
    }
}
