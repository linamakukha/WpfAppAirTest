using System.Windows;
using Microsoft.Win32;

namespace WpfAppAirTest;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const int ThemeDictionaryIndex = 0;

    public bool IsDarkTheme { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        ApplyTheme(IsSystemInDarkMode());
        base.OnStartup(e);
    }

    public void ApplyTheme(bool dark)
    {
        var source = new Uri($"pack://application:,,,/Themes/{(dark ? "Dark" : "Light")}.xaml");
        Resources.MergedDictionaries[ThemeDictionaryIndex] = new ResourceDictionary { Source = source };
        IsDarkTheme = dark;
    }

    private static bool IsSystemInDarkMode()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        return key?.GetValue("AppsUseLightTheme") is 0;
    }
}
