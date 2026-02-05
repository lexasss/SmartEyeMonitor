using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SmartEyeMonitor.ViewModels;

internal class MainVM : INotifyPropertyChanged
{
    public bool IsDebugMode => (App.Current as App)!.IsDebugging;

    public ObservableCollection<Views.Plane> Planes { get; } = [];

    public Services.MappingMode MappingMode
    {
        get => _mapper.Mode;
        set
        {
            _mapper.Mode = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MappingMode)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainVM()
    {
        var app = (App.Current as App)!;
        _mapper = app.Mapper;

        _mapper.PlaneAdded += Mapper_PlaneAdded;

        if (app.IsDebugging)
        {
            foreach (var name in _debugPlaneNames)
            {
                var plane = _mapper.Add(name);
                if (plane != null)
                {
                    var planeView = new Views.Plane(plane);
                    Planes.Add(planeView);
                }
            }

            SEClient.Tcp.Client.SetEmulatedPlanes(_debugPlaneNames.ToArray());
        }
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

    readonly Services.Mapper _mapper;

    private void Mapper_PlaneAdded(object? sender, Models.Plane plane)
    {
        var planeView = new Views.Plane(plane);
        Planes.Add(planeView);
    }
}