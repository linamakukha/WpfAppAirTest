using System.Windows.Input;
using WpfAppAirTest.Mvvm;

namespace WpfAppAirTest.ViewModels;

/// <summary>
/// Root view model of the window: owns the pages and switches between them.
/// </summary>
public sealed class ShellViewModel : ObservableObject
{
    private ObservableObject _currentPage;

    public ShellViewModel()
    {
        _currentPage = Planner;
        ShowPlannerCommand = new RelayCommand(_ => CurrentPage = Planner);
        ShowMoodCommand = new RelayCommand(_ => CurrentPage = Mood);
    }

    public MainViewModel Planner { get; } = new();

    public MoodViewModel Mood { get; } = new();

    public ObservableObject CurrentPage
    {
        get => _currentPage;
        private set => SetField(ref _currentPage, value);
    }

    public ICommand ShowPlannerCommand { get; }

    public ICommand ShowMoodCommand { get; }
}
