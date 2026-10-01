using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using WpfAppAirTest.Models;
using WpfAppAirTest.Mvvm;

namespace WpfAppAirTest.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    // Short pause before moving a toggled item, so the checkbox animation can play first.
    private static readonly TimeSpan MoveDelay = TimeSpan.FromMilliseconds(350);

    private string _newTitle = string.Empty;

    public MainViewModel()
    {
        AddCommand = new RelayCommand(_ => AddTask(), _ => !string.IsNullOrWhiteSpace(NewTitle));
        RemoveCommand = new RelayCommand(p =>
        {
            if (p is TodoItem item)
                RemoveTask(item);
        });

        Pending.CollectionChanged += OnCollectionChanged;
        Done.CollectionChanged += OnCollectionChanged;

        foreach (var title in new[] { "Morning workout", "Reply to emails", "Buy groceries" })
            Track(new TodoItem(title));
    }

    public ObservableCollection<TodoItem> Pending { get; } = [];

    public ObservableCollection<TodoItem> Done { get; } = [];

    public ICommand AddCommand { get; }

    public ICommand RemoveCommand { get; }

    public string NewTitle
    {
        get => _newTitle;
        set => SetField(ref _newTitle, value);
    }

    public string Greeting => DateTime.Now.Hour switch
    {
        < 12 => "Good morning",
        < 18 => "Good afternoon",
        _ => "Good evening"
    };

    public string TodayText => DateTime.Now.ToString("dddd, MMMM d");

    public int TotalCount => Pending.Count + Done.Count;

    public int DoneCount => Done.Count;

    public double Progress => TotalCount == 0 ? 0 : (double)DoneCount / TotalCount;

    public string ProgressPercentText => $"{Math.Round(Progress * 100)}%";

    public string ProgressText => TotalCount switch
    {
        0 => "No tasks yet",
        _ when DoneCount == TotalCount => "All done - nice work!",
        _ => $"{DoneCount} of {TotalCount} tasks done"
    };

    public bool IsEmpty => TotalCount == 0;

    public bool HasDone => DoneCount > 0;

    private void AddTask()
    {
        Track(new TodoItem(NewTitle.Trim()));
        NewTitle = string.Empty;
    }

    private void Track(TodoItem item)
    {
        item.PropertyChanged += OnItemPropertyChanged;
        Pending.Add(item);
    }

    private void RemoveTask(TodoItem item)
    {
        item.PropertyChanged -= OnItemPropertyChanged;
        if (!Pending.Remove(item))
            Done.Remove(item);
    }

    private async void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not TodoItem item || e.PropertyName != nameof(TodoItem.IsDone))
            return;

        await Task.Delay(MoveDelay);

        // The state may have flipped again (or the item was removed) during the delay,
        // so only move it if it is still sitting in the wrong list.
        var (from, to) = item.IsDone ? (Pending, Done) : (Done, Pending);
        if (from.Remove(item))
            to.Add(item);
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(TotalCount));
        OnPropertyChanged(nameof(DoneCount));
        OnPropertyChanged(nameof(Progress));
        OnPropertyChanged(nameof(ProgressPercentText));
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(HasDone));
    }
}
