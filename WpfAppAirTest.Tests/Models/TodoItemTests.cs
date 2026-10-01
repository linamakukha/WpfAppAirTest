using System.ComponentModel;
using WpfAppAirTest.Models;

namespace WpfAppAirTest.Tests.Models;

public class TodoItemTests
{
    [Fact]
    public void Constructor_SetsTitle()
    {
        var item = new TodoItem("Buy milk");

        Assert.Equal("Buy milk", item.Title);
    }

    [Fact]
    public void NewItem_IsNotDone()
    {
        var item = new TodoItem("Buy milk");

        Assert.False(item.IsDone);
    }

    [Fact]
    public void IsDone_CanBeSetToTrue()
    {
        var item = new TodoItem("Buy milk") { IsDone = true };

        Assert.True(item.IsDone);
    }

    [Fact]
    public void IsDone_Change_RaisesPropertyChanged()
    {
        var item = new TodoItem("Buy milk");
        var raised = new List<string?>();
        item.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        item.IsDone = true;

        Assert.Equal([nameof(TodoItem.IsDone)], raised);
    }

    [Fact]
    public void Item_ImplementsINotifyPropertyChanged()
    {
        Assert.IsAssignableFrom<INotifyPropertyChanged>(new TodoItem("x"));
    }
}
