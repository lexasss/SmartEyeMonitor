using System.ComponentModel;
using System.Windows;

namespace SmartEyeMonitor.ViewModels;

internal class ConnectVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Host
    {
        get => Properties.Settings.Default.Host;
        set
        {
            Properties.Settings.Default.Host = value;
            Properties.Settings.Default.Save();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Host)));
        }
    }

    public ushort Port
    {
        get => Properties.Settings.Default.Port;
        set
        {
            Properties.Settings.Default.Port = value;
            Properties.Settings.Default.Save();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Port)));
        }
    }

    public SmartEyeTools.IntersectionSource IntersectionSource
    {
        get => SmartEyeTools.Options.Instance.IntersectionSource;
        set
        {
            SmartEyeTools.Options.Instance.IntersectionSource = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IntersectionSource)));
        }
    }

    public bool IsFiltered
    {
        get => SmartEyeTools.Options.Instance.IntersectionSourceFiltered;
        set
        {
            SmartEyeTools.Options.Instance.IntersectionSourceFiltered = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFiltered)));
        }
    }

    public bool IsConnecting
    {
        get => field;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConnecting)));
        }
    }

    public bool Validate()
    {
        if (Host != "localhost" && Host.Split('.').Where(p => byte.TryParse(p, out byte _)).Count() != 4)
        {
            ShowError("IP is not valid");
            return false;
        }

        if (Port < 1024)
        {
            ShowError("Port is not valid");
            return false;
        }

        return true;
    }

    public async Task<bool> ConnectToEyeTracker()
    {
        IsConnecting = true;

        var eyeTracker = new SmartEyeTools.Client();
        await eyeTracker.Connect(Host, Port, (App.Current as App)!.IsDebugging);

        if (eyeTracker.IsConnected)
        {
            (App.Current as App)!.EyeTracker = eyeTracker;
        }
        else
        {
            eyeTracker.Dispose();
            eyeTracker = null;

            ShowError("Cannot connect to SmartEye");
        }

        IsConnecting = false;

        return eyeTracker != null;
    }

    // Internal

    private static void ShowError(string msg)
    {
        MessageBox.Show(msg, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
