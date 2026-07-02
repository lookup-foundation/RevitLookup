// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using RevitLookup.Common.Utils;
using RevitLookup.UI.Framework.Controls.ColorPicker;
using Wpf.Ui.Controls;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Drawing.ColorConverter;
using Point = System.Windows.Point;
using TextBox = System.Windows.Controls.TextBox;

// ReSharper disable once CheckNamespace
namespace RevitLookup.UI.Framework.Controls;

public sealed partial class ColorPickerControl
{
    private double _currH = 360;
    private double _currS = 1;
    private double _currV = 1;
    private bool _ignoreHexChanges;
    private bool _ignoreRgbChanges;
    private bool _ignoreGradientsChanges;
    private bool _isCollapsed = true;
    private Color _originalColor;
    private Color _currentColor;

    private static readonly Regex HexColorRegex = new("^#?([0-9A-Fa-f]{3}){1,2}$", RegexOptions.Compiled);
    private static readonly Regex ShortHexColorRegex = new("^#?([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])$", RegexOptions.Compiled);
    private static readonly ColorConverter DrawingColorConverter = new();

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor),
        typeof(Color),
        typeof(ColorPickerControl),
        new FrameworkPropertyMetadata(Color.FromArgb(0, 0, 0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedColorPropertyChanged));

    public ColorPickerControl()
    {
        InitializeComponent();
        UpdateHueGradient(1, 1);
    }

    public Color SelectedColor
    {
        get => (Color) GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    private static void SelectedColorPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorPickerControl) dependencyObject;
        var newColor = (Color) e.NewValue;

        control._originalColor = control._currentColor = newColor;
        var newColorBackground = new SolidColorBrush(newColor);
        control.CurrentColorButton.Background = newColorBackground;

        control._ignoreHexChanges = true;
        control._ignoreRgbChanges = true;

        control.HexCode.Text = ColorToHex(newColor);
        control.RNumberBox.Value = newColor.R;
        control.GNumberBox.Value = newColor.G;
        control.BNumberBox.Value = newColor.B;
        control.SetColorFromTextBoxes(System.Drawing.Color.FromArgb(newColor.R, newColor.G, newColor.B));

        control._ignoreRgbChanges = false;
        control._ignoreHexChanges = false;

        var hsv = ColorFormatUtils.ConvertToHsvColor(System.Drawing.Color.FromArgb(newColor.R, newColor.G, newColor.B));
        SetColorVariationsForCurrentColor(dependencyObject, hsv);
    }

    private void UpdateHueGradient(double saturation, double value)
    {
        var g6 = HsvColor.HueSpectrum(saturation, value);

        var gradientBrush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 0)
        };

        for (var i = 0; i < g6.Length; i++)
        {
            var stop = new GradientStop(g6[i], i * 0.16);
            gradientBrush.GradientStops.Add(stop);
        }

        HueGradientSlider.Background = gradientBrush;
    }

    private static void SetColorVariationsForCurrentColor(DependencyObject d, (double Hue, double Saturation, double Value) hsv)
    {
        var hueCoefficient = 0;
        var hueCoefficient2 = 0;
        if (1 - hsv.Value < 0.15)
        {
            hueCoefficient = 1;
        }

        if (hsv.Value - 0.3 < 0)
        {
            hueCoefficient2 = 1;
        }

        var s = hsv.Saturation;
        var control = (ColorPickerControl) d;

        control.ColorVariation1Button.Background = new SolidColorBrush(HsvColor.RgbFromHsv(Math.Min(hsv.Hue + (hueCoefficient * 8), 360), s, Math.Min(hsv.Value + 0.3, 1)));
        control.ColorVariation2Button.Background = new SolidColorBrush(HsvColor.RgbFromHsv(Math.Min(hsv.Hue + (hueCoefficient * 4), 360), s, Math.Min(hsv.Value + 0.15, 1)));

        control.ColorVariation3Button.Background = new SolidColorBrush(HsvColor.RgbFromHsv(Math.Max(hsv.Hue - (hueCoefficient2 * 4), 0), s, Math.Max(hsv.Value - 0.2, 0)));
        control.ColorVariation4Button.Background = new SolidColorBrush(HsvColor.RgbFromHsv(Math.Max(hsv.Hue - (hueCoefficient2 * 8), 0), s, Math.Max(hsv.Value - 0.3, 0)));
    }

    private void UpdateValueColorGradient(double posX)
    {
        ValueGradientSlider.Value = posX;

        _currV = posX / ValueGradientSlider.Maximum;

        UpdateHueGradient(_currS, _currV);

        SaturationStartColor.Color = HsvColor.RgbFromHsv(_currH, 0f, _currV);
        SaturationStopColor.Color = HsvColor.RgbFromHsv(_currH, 1f, _currV);
    }

    private void UpdateSaturationColorGradient(double posX)
    {
        SaturationGradientSlider.Value = posX;

        _currS = posX / HueGradientSlider.Maximum;

        UpdateHueGradient(_currS, _currV);

        ValueStartColor.Color = HsvColor.RgbFromHsv(_currH, _currS, 0f);
        ValueStopColor.Color = HsvColor.RgbFromHsv(_currH, _currS, 1f);
    }

    private void UpdateHueColorGradient(double posX)
    {
        HueGradientSlider.Value = posX;
        _currH = posX / HueGradientSlider.Maximum * 360;

        SaturationStartColor.Color = HsvColor.RgbFromHsv(_currH, 0f, _currV);
        SaturationStopColor.Color = HsvColor.RgbFromHsv(_currH, 1f, _currV);

        ValueStartColor.Color = HsvColor.RgbFromHsv(_currH, _currS, 0f);
        ValueStopColor.Color = HsvColor.RgbFromHsv(_currH, _currS, 1f);
    }

    private void UpdateTextBoxesAndCurrentColor(Color currentColor)
    {
        if (!_ignoreHexChanges)
        {
            // Second parameter is set to keep the hashtag if typed by the user before
            HexCode.Text = ColorToHex(currentColor, HexCode.Text);
        }

        if (!_ignoreRgbChanges)
        {
            RNumberBox.Value = currentColor.R;
            GNumberBox.Value = currentColor.G;
            BNumberBox.Value = currentColor.B;
        }

        _currentColor = currentColor;
        CurrentColorButton.Background = new SolidColorBrush(currentColor);
    }

    private void OnCurrentColorButtonClicked(object sender, RoutedEventArgs e)
    {
        ShowDetails();
    }

    private void ShowDetails()
    {
        if (_isCollapsed)
        {
            _isCollapsed = false;

            var resizeColor = new DoubleAnimation(256, new Duration(TimeSpan.FromMilliseconds(250)))
            {
                EasingFunction = new ExponentialEase {EasingMode = EasingMode.EaseInOut}
            };

            var moveColor = new ThicknessAnimation(new Thickness(0), new Duration(TimeSpan.FromMilliseconds(250)))
            {
                EasingFunction = new ExponentialEase {EasingMode = EasingMode.EaseInOut}
            };

            CurrentColorButton.BeginAnimation(WidthProperty, resizeColor);
            CurrentColorButton.BeginAnimation(MarginProperty, moveColor);
            CurrentColorButton.IsEnabled = false;
            DetailsFlyout.IsOpen = true;
        }
    }

    private void HideDetails()
    {
        if (_isCollapsed) return;

        _isCollapsed = true;

        var resizeColor = new DoubleAnimation(165, new Duration(TimeSpan.FromMilliseconds(150)))
        {
            EasingFunction = new ExponentialEase {EasingMode = EasingMode.EaseInOut}
        };

        var moveColor = new ThicknessAnimation(new Thickness(72, 0, 72, 0), new Duration(TimeSpan.FromMilliseconds(150)))
        {
            EasingFunction = new ExponentialEase {EasingMode = EasingMode.EaseInOut}
        };

        CurrentColorButton.BeginAnimation(WidthProperty, resizeColor);
        CurrentColorButton.BeginAnimation(MarginProperty, moveColor);
        CurrentColorButton.IsEnabled = true;
    }

    private void OnOkButtonClicked(object sender, RoutedEventArgs e)
    {
        SelectedColor = _currentColor;
        DetailsFlyout.Hide();
    }

    private void OnDetailsFlyoutClosed(object sender, object e)
    {
        HideDetails();

        // Revert to original color
        var originalColorBackground = new SolidColorBrush(_originalColor);
        CurrentColorButton.Background = originalColorBackground;

        HexCode.Text = ColorToHex(_originalColor);
    }

    private void OnColorVariationButtonClicked(object sender, RoutedEventArgs e)
    {
        var selectedColor = ((SolidColorBrush) ((Button) sender).Background).Color;
        SelectedColor = selectedColor;
    }

    private void OnSaturationGradientSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UpdateSaturationColorGradient(((Slider) sender).Value);
        _ignoreGradientsChanges = true;
        UpdateTextBoxesAndCurrentColor(HsvColor.RgbFromHsv(_currH, _currS, _currV));
        _ignoreGradientsChanges = false;
    }

    private void OnHueGradientSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UpdateHueColorGradient(((Slider) sender).Value);
        _ignoreGradientsChanges = true;
        UpdateTextBoxesAndCurrentColor(HsvColor.RgbFromHsv(_currH, _currS, _currV));
        _ignoreGradientsChanges = false;
    }

    private void OnValueGradientSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UpdateValueColorGradient(((Slider) sender).Value);
        _ignoreGradientsChanges = true;
        UpdateTextBoxesAndCurrentColor(HsvColor.RgbFromHsv(_currH, _currS, _currV));
        _ignoreGradientsChanges = false;
    }

    private void OnHexCodeTextChanged(object sender, TextChangedEventArgs e)
    {
        var newValue = ((TextBox) sender).Text;

        // support hex with 3 and 6 characters and optional with hashtag
        if (!HexColorRegex.IsMatch(newValue))
        {
            return;
        }

        if (_ignoreHexChanges) return;

        // "FormatHexColorString()" is needed to add hashtag if missing and to convert the hex code from three to six characters. Without this we get format exceptions and incorrect color values.
        var color = (System.Drawing.Color) DrawingColorConverter.ConvertFromString(FormatHexColorString(HexCode.Text))!;

        _ignoreHexChanges = true;
        SetColorFromTextBoxes(color);
        _ignoreHexChanges = false;
    }

    private void SetColorFromTextBoxes(System.Drawing.Color color)
    {
        if (!_ignoreGradientsChanges)
        {
            var hsv = ColorFormatUtils.ConvertToHsvColor(color);

            var huePosition = (hsv.Hue / 360) * HueGradientSlider.Maximum;
            var saturationPosition = hsv.Saturation * SaturationGradientSlider.Maximum;
            var valuePosition = hsv.Value * ValueGradientSlider.Maximum;
            UpdateHueColorGradient(huePosition);
            UpdateSaturationColorGradient(saturationPosition);
            UpdateValueColorGradient(valuePosition);
        }

        UpdateTextBoxesAndCurrentColor(Color.FromRgb(color.R, color.G, color.B));
    }

    private static string ColorToHex(Color color, string oldValue = "")
    {
#if NET
        var newHexString = BitConverter.ToString([color.R, color.G, color.B]).Replace("-", string.Empty, StringComparison.InvariantCulture);
#else
        var newHexString = BitConverter.ToString([color.R, color.G, color.B]).Replace("-", string.Empty);
#endif
        newHexString = newHexString.ToLowerInvariant();

        // Return only with hashtag if user typed it before
        var addHashtag = oldValue.StartsWith('#');
        return addHashtag ? "#" + newHexString : newHexString;
    }

    /// <summary>
    /// Formats the hex code string to be accepted by <see cref="System.Drawing.ColorConverter.ConvertFromString(string)"/>. We are adding hashtag at the beginning if needed and convert from three characters to six characters code.
    /// </summary>
    /// <param name="hexCodeText">The string we read from the hex text box.</param>
    /// <returns>Formatted string with hashtag and six characters of hex code.</returns>
    private static string FormatHexColorString(string hexCodeText)
    {
        if (hexCodeText.Length is 3 or 4)
        {
            // Hex with or without hashTag and three characters
            return ShortHexColorRegex.Replace(hexCodeText, "#$1$1$2$2$3$3");
        }

        // Hex with or without hashTag and six characters
        return hexCodeText.StartsWith('#') ? hexCodeText : "#" + hexCodeText;
    }

    private void OnHexCodeGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        ((TextBox) sender).SelectAll();
    }

    private void OnRgbNumberBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_ignoreRgbChanges) return;

        var numberBox = (NumberBox) sender;

        if (!RNumberBox.Value.HasValue) return;
        if (!GNumberBox.Value.HasValue) return;
        if (!BNumberBox.Value.HasValue) return;

        var r = numberBox.Name == "RNumberBox" ? GetValueFromNumberBox(numberBox) : (byte) RNumberBox.Value;
        var g = numberBox.Name == "GNumberBox" ? GetValueFromNumberBox(numberBox) : (byte) GNumberBox.Value;
        var b = numberBox.Name == "BNumberBox" ? GetValueFromNumberBox(numberBox) : (byte) BNumberBox.Value;

        _ignoreRgbChanges = true;
        SetColorFromTextBoxes(System.Drawing.Color.FromArgb(r, g, b));
        _ignoreRgbChanges = false;
    }

    /// <summary>
    /// NumberBox provides value only after it has been validated - happens after pressing enter or leaving this control.
    /// However, we need to get value immediately after the underlying textbox value changes
    /// </summary>
    /// <param name="numberBox">numberBox control which value we want to get</param>
    /// <returns>Validated value as per numberbox conditions, if content is invalid it returns previous value</returns>
    private static byte GetValueFromNumberBox(NumberBox numberBox)
    {
        if (!numberBox.Value.HasValue) return byte.MinValue;

        var parsedValue = ParseDouble(numberBox.Text);
        if (!parsedValue.HasValue) return (byte) numberBox.Value;

        var parsedValueByte = (byte) parsedValue;

        if (parsedValueByte >= numberBox.Minimum && parsedValueByte <= numberBox.Maximum)
        {
            return parsedValueByte;
        }

        // not valid input, return previous value
        return (byte) numberBox.Value;
    }

    public static double? ParseDouble(string text)
    {
        if (double.TryParse(text, out var result))
        {
            return result;
        }

        return null;
    }
}