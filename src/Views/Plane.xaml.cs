using System.Windows.Controls;

namespace SmartEyeMonitor.Views;

public partial class Plane : UserControl
{
    public ViewModels.PlaneVM ViewModel { get; }

    public Plane(Models.Plane plane)
    {
        ViewModel = new ViewModels.PlaneVM(plane);

        InitializeComponent();
    }

    // UI

    private void UserControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        ViewModel.IsEnabled = !ViewModel.IsEnabled;
    }
}
