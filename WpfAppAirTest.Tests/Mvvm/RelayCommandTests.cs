using WpfAppAirTest.Mvvm;

namespace WpfAppAirTest.Tests.Mvvm;

public class RelayCommandTests
{
    [Fact]
    public void Execute_InvokesActionWithParameter()
    {
        object? received = null;
        var command = new RelayCommand(p => received = p);

        command.Execute("param");

        Assert.Equal("param", received);
    }

    [Fact]
    public void CanExecute_WithoutPredicate_ReturnsTrue()
    {
        var command = new RelayCommand(_ => { });

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void CanExecute_WithPredicate_PassesParameterAndReturnsResult()
    {
        object? received = null;
        var command = new RelayCommand(_ => { }, p =>
        {
            received = p;
            return true;
        });

        Assert.True(command.CanExecute("ok"));
        Assert.Equal("ok", received);
    }
}
