using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartEyeMonitor.Views;

public partial class Connect : Page
{
    public event EventHandler? Connected;

    public Connect()
    {
        InitializeComponent();
    }

    // Internal

    // UI events

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        window.KeyDown += Page_KeyDown;
    }

    private void Page_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F2)
        {
            (App.Current as App)!.IsDebugging = true;
            Connect_Click(this, new RoutedEventArgs());
        }
    }

    private async void Connect_Click(object? _, RoutedEventArgs e)
    {
        var vm = (((FrameworkElement)Content).DataContext as ViewModels.ConnectVM)!;
        if (!vm.Validate())
        {
            return;
        }

        var isConnected = await vm.ConnectToEyeTracker();
        if (isConnected)
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }
    }
}
