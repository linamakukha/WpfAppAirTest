namespace WpfAppAirTest.Models;

public sealed record MoodEntry(Mood Mood, DateTime Time)
{
    public string TimeText => Time.ToString("t");
}
