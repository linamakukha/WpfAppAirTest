using WpfAppAirTest.Mvvm;

namespace WpfAppAirTest.Models;

public sealed class TodoItem(string title) : ObservableObject
{
    private bool _isDone;

    public string Title { get; } = title;

    public bool IsDone
    {
        get => _isDone;
        set => SetField(ref _isDone, value);
    }
}
