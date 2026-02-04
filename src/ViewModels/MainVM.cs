using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SmartEyeMonitor.ViewModels;

internal class MainVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<Views.Plane> Planes { get; } = [];

    public Services.MappingMode MappingMode
    {
        get => (App.Current as App)!.Mapper.Mode;
        set
        {
            (App.Current as App)!.Mapper.Mode = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MappingMode)));
        }
    }

    public bool IsDebugMode => (App.Current as App)!.IsDebugging;

    public MainVM()
    {
        var app = (App.Current as App)!;
        app.DebugModeChanged += App_DebugModeChanged;
        if (app.IsDebugging)
        {
            App_DebugModeChanged(this, true);
        }

        var mapper = (App.Current as App)!.Mapper;
        mapper.PlaneAdded += (s, plane) =>
        {
            var planeView = new Views.Plane(plane);
            Planes.Add(planeView);
        };
    }

    public void AddRandomPlane()
    {
        var name = $"Plane {new Random().Next(1000)}";
        if (Planes.FirstOrDefault(p => p.Name == name) != null)
        {
            return;
        }

        _debugPlaneNames.Add(name);
        SEClient.Tcp.Client.SetEmulatedPlanes(_debugPlaneNames.ToArray());
    }

    // Internal

    readonly List<string> _debugPlaneNames =
    [
        "Windshield",
        "Left Mirror",
        "Left Dashboard",
        "Rear View",
        "Central Console",
        "Right Mirror",
    ];

    private void App_DebugModeChanged(object? sender, bool isDebugMode)
    {
        if (isDebugMode)
        {
            var mapper = (App.Current as App)!.Mapper;
            foreach (var name in _debugPlaneNames)
            {
                var plane = mapper.Add(name);
                if (plane != null)
                {
                    var planeView = new Views.Plane(plane);
                    Planes.Add(planeView);
                }
            }

            SEClient.Tcp.Client.SetEmulatedPlanes(_debugPlaneNames.ToArray());
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDebugMode)));
    }
}