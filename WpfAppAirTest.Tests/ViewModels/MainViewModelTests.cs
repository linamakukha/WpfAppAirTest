using WpfAppAirTest.ViewModels;

namespace WpfAppAirTest.Tests.ViewModels;

public class MainViewModelTests
{
    // The view model moves a toggled item after a short delay, so poll until the move happens.
    private static async Task WaitUntilAsync(Func<bool> condition, int timeoutMs = 3000)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
        while (!condition() && DateTime.UtcNow < deadline)
            await Task.Delay(25);

        Assert.True(condition(), "Condition was not met within the timeout.");
    }

    [Fact]
    public void Constructor_SeedsThreePendingTasks()
    {
        var vm = new MainViewModel();

        Assert.Equal(["Morning workout", "Reply to emails", "Buy groceries"], vm.Pending.Select(i => i.Title));
        Assert.All(vm.Pending, i => Assert.False(i.IsDone));
        Assert.Empty(vm.Done);
    }

    [Fact]
    public void Constructor_InitialState_IsConsistent()
    {
        var vm = new MainViewModel();

        Assert.Equal(string.Empty, vm.NewTitle);
        Assert.Equal(3, vm.TotalCount);
        Assert.Equal(0, vm.DoneCount);
        Assert.Equal(0, vm.Progress);
        Assert.Equal("0%", vm.ProgressPercentText);
        Assert.Equal("0 of 3 tasks done", vm.ProgressText);
        Assert.False(vm.IsEmpty);
        Assert.False(vm.HasDone);
    }

    [Fact]
    public void NewTitle_Set_UpdatesValueAndRaisesPropertyChanged()
    {
        var vm = new MainViewModel();
        var raised = new List<string?>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        vm.NewTitle = "Read a book";

        Assert.Equal("Read a book", vm.NewTitle);
        Assert.Contains(nameof(MainViewModel.NewTitle), raised);
    }

    [Fact]
    public void AddCommand_CanExecute_WhenTitleIsFilled()
    {
        var vm = new MainViewModel { NewTitle = "Read a book" };

        Assert.True(vm.AddCommand.CanExecute(null));
    }

    [Fact]
    public void AddCommand_Execute_AddsTrimmedTaskToPending()
    {
        var vm = new MainViewModel { NewTitle = "  Read a book  " };

        vm.AddCommand.Execute(null);

        Assert.Equal(4, vm.Pending.Count);
        Assert.Equal("Read a book", vm.Pending[^1].Title);
        Assert.False(vm.Pending[^1].IsDone);
    }

    [Fact]
    public void AddCommand_Execute_ClearsNewTitle()
    {
        var vm = new MainViewModel { NewTitle = "Read a book" };

        vm.AddCommand.Execute(null);

        Assert.Equal(string.Empty, vm.NewTitle);
    }

    [Fact]
    public void AddCommand_Execute_UpdatesCountersAndRaisesNotifications()
    {
        var vm = new MainViewModel { NewTitle = "Read a book" };
        var raised = new HashSet<string?>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        vm.AddCommand.Execute(null);

        Assert.Equal(4, vm.TotalCount);
        Assert.Equal("0 of 4 tasks done", vm.ProgressText);
        Assert.Superset(
            new HashSet<string?>
            {
                nameof(MainViewModel.TotalCount),
                nameof(MainViewModel.DoneCount),
                nameof(MainViewModel.Progress),
                nameof(MainViewModel.ProgressPercentText),
                nameof(MainViewModel.ProgressText),
                nameof(MainViewModel.IsEmpty),
                nameof(MainViewModel.HasDone)
            },
            raised);
    }

    [Fact]
    public void RemoveCommand_Execute_RemovesPendingTask()
    {
        var vm = new MainViewModel();
        var item = vm.Pending[0];

        vm.RemoveCommand.Execute(item);

        Assert.DoesNotContain(item, vm.Pending);
        Assert.Equal(2, vm.TotalCount);
    }

    [Fact]
    public async Task RemoveCommand_Execute_RemovesDoneTask()
    {
        var vm = new MainViewModel();
        var item = vm.Pending[0];
        item.IsDone = true;
        await WaitUntilAsync(() => vm.Done.Contains(item));

        vm.RemoveCommand.Execute(item);

        Assert.DoesNotContain(item, vm.Done);
        Assert.Equal(2, vm.TotalCount);
        Assert.False(vm.HasDone);
    }

    [Fact]
    public void RemoveCommand_Execute_AllTasks_MakesListEmpty()
    {
        var vm = new MainViewModel();

        foreach (var item in vm.Pending.ToList())
            vm.RemoveCommand.Execute(item);

        Assert.True(vm.IsEmpty);
        Assert.Equal(0, vm.TotalCount);
        Assert.Equal(0, vm.Progress);
        Assert.Equal("0%", vm.ProgressPercentText);
        Assert.Equal("No tasks yet", vm.ProgressText);
    }

    [Fact]
    public async Task MarkingTaskDone_MovesItToDone()
    {
        var vm = new MainViewModel();
        var item = vm.Pending[0];

        item.IsDone = true;
        await WaitUntilAsync(() => vm.Done.Contains(item));

        Assert.DoesNotContain(item, vm.Pending);
        Assert.Equal(1, vm.DoneCount);
        Assert.True(vm.HasDone);
    }

    [Fact]
    public async Task MarkingTaskDone_UpdatesProgress()
    {
        var vm = new MainViewModel();
        var item = vm.Pending[0];

        item.IsDone = true;
        await WaitUntilAsync(() => vm.Done.Contains(item));

        Assert.Equal(1.0 / 3, vm.Progress, 5);
        Assert.Equal("33%", vm.ProgressPercentText);
        Assert.Equal("1 of 3 tasks done", vm.ProgressText);
    }

    [Fact]
    public async Task MarkingAllTasksDone_ShowsAllDoneMessage()
    {
        var vm = new MainViewModel();

        foreach (var item in vm.Pending.ToList())
            item.IsDone = true;
        await WaitUntilAsync(() => vm.Done.Count == 3);

        Assert.Empty(vm.Pending);
        Assert.Equal(1, vm.Progress);
        Assert.Equal("100%", vm.ProgressPercentText);
        Assert.Equal("All done - nice work!", vm.ProgressText);
    }

    [Fact]
    public async Task UnmarkingDoneTask_MovesItBackToPending()
    {
        var vm = new MainViewModel();
        var item = vm.Pending[0];
        item.IsDone = true;
        await WaitUntilAsync(() => vm.Done.Contains(item));

        item.IsDone = false;
        await WaitUntilAsync(() => vm.Pending.Contains(item));

        Assert.Empty(vm.Done);
        Assert.False(vm.HasDone);
        Assert.Equal(3, vm.Pending.Count);
    }

    [Fact]
    public void Greeting_MatchesCurrentTimeOfDay()
    {
        var vm = new MainViewModel();
        var expected = DateTime.Now.Hour switch
        {
            < 12 => "Good morning",
            < 18 => "Good afternoon",
            _ => "Good evening"
        };

        Assert.Equal(expected, vm.Greeting);
    }

    [Fact]
    public void TodayText_FormatsCurrentDate()
    {
        var vm = new MainViewModel();

        Assert.Equal(DateTime.Now.ToString("dddd, MMMM d"), vm.TodayText);
    }
}
