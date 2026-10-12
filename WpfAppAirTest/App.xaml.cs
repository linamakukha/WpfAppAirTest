using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Theming;

namespace WpfAppAirTest;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string LightBaseColor = "Light";
    private const string DarkBaseColor = "Dark";

    // Brand violet; a lighter shade on dark backgrounds keeps enough contrast.
    private static readonly Color LightAccent = Color.FromRgb(0x7C, 0x3A, 0xED);
    private static readonly Color DarkAccent = Color.FromRgb(0xA7, 0x8B, 0xFA);

    private Theme _lightTheme = null!;
    private Theme _darkTheme = null!;
    private ResourceDictionary _palette = null!;

    public bool IsDarkTheme { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        _lightTheme = CreateTheme(LightBaseColor, LightAccent);
        _darkTheme = CreateTheme(DarkBaseColor, DarkAccent);
        _palette = Resources.MergedDictionaries.First(d => d.Source?.OriginalString.EndsWith("Themes/Light.xaml") == true);

        ApplyTheme(WindowsThemeHelper.GetWindowsBaseColor() == DarkBaseColor);
        base.OnStartup(e);
    }

    public void ApplyTheme(bool dark)
    {
        ThemeManager.Current.ChangeTheme(this, dark ? _darkTheme : _lightTheme);

        var palette = new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/Themes/{(dark ? "Dark" : "Light")}.xaml")
        };
        Resources.MergedDictionaries.Remove(_palette);
        Resources.MergedDictionaries.Add(palette);
        _palette = palette;

        IsDarkTheme = dark;
    }

    private static Theme CreateTheme(string baseColor, Color accent)
    {
        var theme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme(baseColor, accent, false, MahAppsLibraryThemeProvider.DefaultInstance)
                    ?? throw new InvalidOperationException($"Could not generate the {baseColor} theme.");
        return ThemeManager.Current.AddTheme(theme);
    }
}
