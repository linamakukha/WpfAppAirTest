using WpfAppAirTest.Mvvm;

namespace WpfAppAirTest.Tests.Mvvm;

public class ObservableObjectTests
{
    private sealed class Sample : ObservableObject
    {
        private int _value;

        public int Value
        {
            get => _value;
            set => LastSetResult = SetField(ref _value, value);
        }

        public bool LastSetResult { get; private set; }

        public void Notify(string name) => OnPropertyChanged(name);
    }

    [Fact]
    public void SetField_NewValue_UpdatesFieldAndReturnsTrue()
    {
        var sample = new Sample();

        sample.Value = 42;

        Assert.Equal(42, sample.Value);
        Assert.True(sample.LastSetResult);
    }

    [Fact]
    public void SetField_NewValue_RaisesPropertyChangedWithCallerName()
    {
        var sample = new Sample();
        string? raisedName = null;
        sample.PropertyChanged += (_, e) => raisedName = e.PropertyName;

        sample.Value = 1;

        Assert.Equal(nameof(Sample.Value), raisedName);
    }

    [Fact]
    public void OnPropertyChanged_RaisesEventWithGivenNameAndSender()
    {
        var sample = new Sample();
        object? sender = null;
        string? raisedName = null;
        sample.PropertyChanged += (s, e) =>
        {
            sender = s;
            raisedName = e.PropertyName;
        };

        sample.Notify("Anything");

        Assert.Same(sample, sender);
        Assert.Equal("Anything", raisedName);
    }
}
