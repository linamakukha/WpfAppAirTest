using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using WpfAppAirTest.ViewModels;

namespace WpfAppAirTest;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string MoonGlyph = "";
    private const string SunGlyph = "";
    private const string MaximizeGlyph = "";
    private const string RestoreGlyph = "";

    private const int DwmwaWindowCornerPreference = 33;
    private const int DwmwcpRound = 2;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        UpdateThemeGlyph();
        Loaded += (_, _) => NewTaskBox.Focus();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        // Ask Windows 11 to round the corners of the custom-chrome window (no-op on older Windows).
        var preference = DwmwcpRound;
        DwmSetWindowAttribute(new WindowInteropHelper(this).Handle, DwmwaWindowCornerPreference, ref preference, sizeof(int));
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);

        // A maximized custom-chrome window overhangs the screen by the resize border, so pad it back in.
        var maximized = WindowState == WindowState.Maximized;
        RootGrid.Margin = maximized ? new Thickness(8) : new Thickness(0);
        MaximizeButton.Content = maximized ? RestoreGlyph : MaximizeGlyph;
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        var app = (App)Application.Current;
        app.ApplyTheme(!app.IsDarkTheme);
        UpdateThemeGlyph();
    }

    private void UpdateThemeGlyph() =>
        ThemeButton.Content = ((App)Application.Current).IsDarkTheme ? SunGlyph : MoonGlyph;

    private void MinimizeButton_Click(object sender, RoutedEventArgs e) => SystemCommands.MinimizeWindow(this);

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
            SystemCommands.RestoreWindow(this);
        else
            SystemCommands.MaximizeWindow(this);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => SystemCommands.CloseWindow(this);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
}
