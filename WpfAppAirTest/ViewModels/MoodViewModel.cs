using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using WpfAppAirTest.Models;
using WpfAppAirTest.Mvvm;

namespace WpfAppAirTest.ViewModels;

public sealed class MoodViewModel : ObservableObject
{
    private static readonly Color CustomMoodColor = Color.FromRgb(0x8B, 0x5C, 0xF6);

    private Mood? _selectedMood;
    private string _customMood = string.Empty;

    public MoodViewModel()
    {
        SaveCommand = new RelayCommand(_ => Save(), _ => SelectedMood is not null || !string.IsNullOrWhiteSpace(CustomMood));

        Entries.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(LatestEntry));
            OnPropertyChanged(nameof(HasEntries));
            OnPropertyChanged(nameof(SummaryTitle));
            OnPropertyChanged(nameof(SummarySubtitle));
        };
    }

    public IReadOnlyList<Mood> Presets { get; } =
    [
        new("Happy", Color.FromRgb(0xF5, 0x9E, 0x0B)),
        new("Funny", Color.FromRgb(0xF9, 0x73, 0x16)),
        new("Excited", Color.FromRgb(0xEC, 0x48, 0x99)),
        new("Calm", Color.FromRgb(0x14, 0xB8, 0xA6)),
        new("Grateful", Color.FromRgb(0x22, 0xC5, 0x5E)),
        new("Focused", Color.FromRgb(0x3B, 0x82, 0xF6)),
        new("Tired", Color.FromRgb(0x64, 0x74, 0x8B)),
        new("Anxious", Color.FromRgb(0xA8, 0x55, 0xF7)),
        new("Sad", Color.FromRgb(0x63, 0x66, 0xF1)),
        new("Frustrated", Color.FromRgb(0xEF, 0x44, 0x44))
    ];

    // Newest first.
    public ObservableCollection<MoodEntry> Entries { get; } = [];

    public ICommand SaveCommand { get; }

    // A preset bubble and the manual input are mutually exclusive: picking one clears the other.
    public Mood? SelectedMood
    {
        get => _selectedMood;
        set
        {
            if (SetField(ref _selectedMood, value) && value is not null)
                CustomMood = string.Empty;
        }
    }

    public string CustomMood
    {
        get => _customMood;
        set
        {
            if (SetField(ref _customMood, value) && !string.IsNullOrWhiteSpace(value))
                SelectedMood = null;
        }
    }

    public MoodEntry? LatestEntry => Entries.FirstOrDefault();

    public bool HasEntries => Entries.Count > 0;

    public string SummaryTitle => LatestEntry is { } entry
        ? $"Today's mood: {entry.Mood.Name}"
        : "How are you feeling today?";

    public string SummarySubtitle => LatestEntry is { } entry
        ? $"Checked in at {entry.TimeText} - tap to update"
        : "Tap to check in";

    private void Save()
    {
        var mood = SelectedMood ?? new Mood(CustomMood.Trim(), CustomMoodColor);
        Entries.Insert(0, new MoodEntry(mood, DateTime.Now));

        SelectedMood = null;
        CustomMood = string.Empty;
    }
}
