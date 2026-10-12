using System.Windows;
using MahApps.Metro.Controls;
using WpfAppAirTest.ViewModels;

namespace WpfAppAirTest;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
    private const string MoonGlyph = "";
    private const string SunGlyph = "";

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new ShellViewModel();
        UpdateThemeGlyph();
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        var app = (App)Application.Current;
        app.ApplyTheme(!app.IsDarkTheme);
        UpdateThemeGlyph();
    }

    private void UpdateThemeGlyph() =>
        ThemeButton.Content = ((App)Application.Current).IsDarkTheme ? SunGlyph : MoonGlyph;
}
