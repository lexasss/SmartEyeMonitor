using SmartEyeMonitor.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartEyeMonitor.Views;

public partial class Main : Page
{
    public Main()
    {
        InitializeComponent();
    }

    // UI events

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        window.KeyDown += Page_KeyDown;
    }

    private void Page_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F5)
        {
            (((FrameworkElement)Content).DataContext as MainVM)!.AddRandomPlane();
        }
    }
}
