using System.Globalization;
using System.Windows;

namespace SmartEyeMonitor;

public partial class App : Application
{
    public bool IsDebugging { get; set; } = false;

    public Services.Mapper Mapper { get; } = new();

    public SEClient.Tcp.Client? EyeTracker
    {
        get => field;
        set
        {
            if (value != null && field != value)
            {
                value.Disconnected -= TcpClient_Disconnected;
                value.Sample -= TcpClient_Sample;
            }

            field = value;

            if (field != null)
            {
                field.Disconnected += TcpClient_Disconnected;
                field.Sample += TcpClient_Sample;
            }
        }
    }

    public App() : base()
    {
        SEClient.Options.Load(SE_CLIENT_OPTIONS_FILENAME);

        // Set the US-culture across the application to avoid decimal point parsing/logging issues
        var culture = CultureInfo.GetCultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        Exit += App_Exit;
    }

    // Internal

    const string SE_CLIENT_OPTIONS_FILENAME = "se_client_options.json";

    // Handlers

    private void TcpClient_Disconnected(object? sender, EventArgs e)
    {
        try
        {
            Dispatcher.Invoke(() => { /* eg., save log file */ });
        }
        catch (TaskCanceledException) { }
    }

    private void TcpClient_Sample(object? sender, SEClient.Tcp.Data.Sample sample)
    {
        try
        {
            Dispatcher.Invoke(() =>
            {
                Mapper.Feed(sample);
            });
        }
        catch (TaskCanceledException) { }
    }

    private void App_Exit(object sender, ExitEventArgs e)
    {
        if (EyeTracker?.IsConnected ?? false)
        {
            EyeTracker.Stop();
        }

        EyeTracker?.Dispose();
    }
}
