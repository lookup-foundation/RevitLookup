using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RevitLookup.Abstractions.Services.Appearance;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.Services.Settings;
using RevitLookup.UI.Playground.Views.Pages;
using RevitLookup.UI.Playground.Views.Pages.BasicInput;
using RevitLookup.UI.Playground.Views.Pages.Collections;
using RevitLookup.UI.Playground.Views.Pages.DesignGuidance;
using RevitLookup.UI.Playground.Views.Pages.DialogsAndFlyouts;
using RevitLookup.UI.Playground.Views.Pages.Layout;
using RevitLookup.UI.Playground.Views.Pages.Navigation;
using RevitLookup.UI.Playground.Views.Pages.ProjectControls;
using RevitLookup.UI.Playground.Views.Pages.Text;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using DataGrid = System.Windows.Controls.DataGrid;
using ListView = System.Windows.Controls.ListView;
using PasswordBox = Wpf.Ui.Controls.PasswordBox;
using Separator = System.Windows.Controls.Separator;
using TextBlock = Wpf.Ui.Controls.TextBlock;
using TextBox = Wpf.Ui.Controls.TextBox;
using NumberBox = Wpf.Ui.Controls.NumberBox;
using ToggleButton = System.Windows.Controls.Primitives.ToggleButton;
using TreeView = System.Windows.Controls.TreeView;

namespace RevitLookup.UI.Playground.ViewModels;

[UsedImplicitly]
public sealed class PlaygroundViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly INotificationService _notificationService;
    private readonly IThemeWatcherService _themeService;

    public PlaygroundViewModel(ISettingsService settingsService, INotificationService notificationService, IThemeWatcherService themeService)
    {
        _settingsService = settingsService;
        _notificationService = notificationService;
        _themeService = themeService;

        MenuItems =
        [
            new NavigationViewItem
            {
                Content = "Home",
                Icon = new FontIcon { Glyph = "\ue80f", FontSize = 16 },
                TargetPageType = typeof(DashboardPage)
            },
            new NavigationViewItem
            {
                Content = "Design guidance",
                Icon = new FontIcon { Glyph = "\uEB3C", FontSize = 16 },
                MenuItemsSource = new object[]
                {
                    new NavigationViewItem("Typography", SymbolRegular.TextFont24, typeof(TypographyPage)),
                    new NavigationViewItem("Colors", SymbolRegular.Color24, typeof(ColorsPage)),
                    new NavigationViewItem("Segoe icons", SymbolRegular.Diversity28, typeof(FontIconsPage)),
                    new NavigationViewItem("Fluent icons", SymbolRegular.Diversity28, typeof(SymbolIconsPage))
                }
            },
            new NavigationViewItem
            {
                Content = "Project controls",
                Icon = new FontIcon { Glyph = "\uEF58", FontSize = 16 },
                MenuItemsSource = new object[]
                {
                    new NavigationViewItem("Chips", typeof(ChipsPage)),
                    new NavigationViewItem("Empty states", typeof(EmptyStatesPage)),
                },
            },
            new NavigationViewItem
            {
                Content = "Basic Input",
                Icon = new FontIcon { Glyph = "\ue73a", FontSize = 16 },
                MenuItemsSource = new object[]
                {
                    new NavigationViewItem(nameof(Anchor), typeof(AnchorPage)),
                    new NavigationViewItem(nameof(Button), typeof(ButtonPage)),
                    new NavigationViewItem(nameof(CheckBox), typeof(CheckBoxPage)),
                    new NavigationViewItem(nameof(ComboBox), typeof(ComboBoxPage)),
                    new NavigationViewItem(nameof(DropDownButton), typeof(DropDownButtonPage)),
                    new NavigationViewItem(nameof(HyperlinkButton), typeof(HyperlinkButtonPage)),
                    new NavigationViewItem(nameof(RadioButton), typeof(RadioButtonPage)),
                    new NavigationViewItem(nameof(Slider), typeof(SliderPage)),
                    new NavigationViewItem(nameof(SplitButton), typeof(SplitButtonPage)),
                    new NavigationViewItem(nameof(ToggleButton), typeof(ToggleButtonPage)),
                    new NavigationViewItem(nameof(ToggleSwitch), typeof(ToggleSwitchPage)),
                }
            },
            new NavigationViewItem
            {
                Content = "Collections",
                Icon = new FontIcon { Glyph = "\uE80A", FontSize = 16 },
                MenuItemsSource = new object[]
                {
                    new NavigationViewItem(nameof(DataGrid), typeof(DataGridPage)),
                    new NavigationViewItem(nameof(ListBox), typeof(ListBoxPage)),
                    new NavigationViewItem(nameof(ListView), typeof(ListViewPage)),
                    new NavigationViewItem(nameof(TreeView), typeof(TreeViewPage)),
                }
            },
            new NavigationViewItem
            {
                Content = "Layout",
                Icon = new FontIcon { Glyph = "\uF246", FontSize = 16 },
                MenuItemsSource = new object[]
                {
                    new NavigationViewItem(nameof(Card), typeof(CardPage)),
                    new NavigationViewItem(nameof(CardAction), typeof(CardActionPage)),
                    new NavigationViewItem(nameof(CardColor), typeof(CardColorPage)),
                    new NavigationViewItem(nameof(CardControl), typeof(CardControlPage)),
                    new NavigationViewItem(nameof(CardExpander), typeof(CardExpanderPage)),
                    new NavigationViewItem(nameof(Separator), typeof(SeparatorPage)),
                }
            },
            new NavigationViewItem
            {
                Content = "Dialogs & flyouts",
                Icon = new FontIcon { Glyph = "\uE8BD", FontSize = 16 },
                MenuItemsSource = new object[]
                {
                    new NavigationViewItem(nameof(Snackbar), typeof(SnackbarPage)),
                    new NavigationViewItem(nameof(Flyout), typeof(FlyoutPage))
                }
            },
            new NavigationViewItem
            {
                Content = "Navigation",
                Icon = new FontIcon { Glyph = "\ue700", FontSize = 16 },
                MenuItemsSource = new object[]
                {
                    new NavigationViewItem(nameof(BreadcrumbBar), typeof(BreadcrumbBarPage)),
                    new NavigationViewItem(nameof(Menu), typeof(MenuPage)),
                    new NavigationViewItem(nameof(TabControl), typeof(TabControlPage))
                }
            },
            new NavigationViewItem
            {
                Content = "Text",
                Icon = new FontIcon { Glyph = "\ue8d2", FontSize = 16 },
                MenuItemsSource = new object[]
                {
                    new NavigationViewItem(nameof(Label), typeof(LabelPage)),
                    new NavigationViewItem(nameof(PasswordBox), typeof(PasswordBoxPage)),
                    new NavigationViewItem(nameof(TextBlock), typeof(TextBlockPage)),
                    new NavigationViewItem(nameof(TextBox), typeof(TextBoxPage)),
                    new NavigationViewItem(nameof(NumberBox), typeof(NumberBoxPage))
                }
            }
        ];

        FooterItems =
        [
            new NavigationViewItem("Switch theme", SymbolRegular.DarkTheme24, null!) { Command = new RelayCommand(SwitchApplicationTheme) },
            new NavigationViewItem("Switch effect", SymbolRegular.Blur24, null!) { Command = new RelayCommand(SwitchBackgroundEffect) },
        ];
    }

    public List<object> MenuItems { get; }
    public List<object> FooterItems { get; }

    private void SwitchApplicationTheme()
    {
        var applicationTheme = ApplicationThemeManager.GetAppTheme();
        if (applicationTheme == ApplicationTheme.Auto)
        {
            applicationTheme = ApplicationThemeManager.GetSystemTheme() switch
            {
                SystemTheme.Light => ApplicationTheme.Light,
                SystemTheme.Dark => ApplicationTheme.Dark,
                _ => ApplicationTheme.Light
            };
        }

        var newTheme = applicationTheme == ApplicationTheme.Light ? ApplicationTheme.Dark : ApplicationTheme.Light;
        _settingsService.ApplicationSettings.Theme = newTheme;
        _themeService.ApplyTheme();

        _notificationService.ShowSuccess("Theme changed", $"The application theme changed: {newTheme}");
    }

    private void SwitchBackgroundEffect()
    {
        var backdropType = _settingsService.ApplicationSettings.Background switch
        {
            WindowBackdropType.None => WindowBackdropType.Mica,
            WindowBackdropType.Mica => WindowBackdropType.Tabbed,
            WindowBackdropType.Tabbed => WindowBackdropType.Acrylic,
            WindowBackdropType.Acrylic => WindowBackdropType.None,
            WindowBackdropType.Auto => WindowBackdropType.None,
            _ => throw new ArgumentOutOfRangeException()
        };

        _settingsService.ApplicationSettings.Background = backdropType;
        _themeService.ApplyTheme();

        _notificationService.ShowSuccess("The background effect changed", $"The application background effect changed: {backdropType}");
    }
}