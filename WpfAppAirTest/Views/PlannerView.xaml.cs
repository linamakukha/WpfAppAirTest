using System.Windows.Controls;

namespace WpfAppAirTest.Views;

public partial class PlannerView : UserControl
{
    public PlannerView()
    {
        InitializeComponent();
        Loaded += (_, _) => NewTaskBox.Focus();
    }
}
