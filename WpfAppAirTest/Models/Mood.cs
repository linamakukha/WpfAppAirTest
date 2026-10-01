using System.Windows.Media;

namespace WpfAppAirTest.Models;

public sealed class Mood
{
    public Mood(string name, Color color)
    {
        Name = name;
        Brush = Freeze(new SolidColorBrush(color));
        SoftBrush = Freeze(new SolidColorBrush(Color.FromArgb(0x2E, color.R, color.G, color.B)));
    }

    public string Name { get; }

    public Brush Brush { get; }

    // Translucent tint of the mood color, readable on both light and dark backgrounds.
    public Brush SoftBrush { get; }

    // Used as the accessible name of the mood bubbles.
    public override string ToString() => Name;

    private static Brush Freeze(Brush brush)
    {
        brush.Freeze();
        return brush;
    }
}
