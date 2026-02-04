using System.Windows;

namespace SmartEyeMonitor;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var connectPage = new Views.Connect();
        connectPage.Connected += (s, e) => Content = new Views.Main();

        Content = connectPage;
    }
}
