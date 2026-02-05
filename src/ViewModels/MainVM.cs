using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SmartEyeMonitor.ViewModels;

internal class MainVM : INotifyPropertyChanged
{
    public bool IsDebugMode => (App.Current as App)!.IsDebugging;

    public ObservableCollection<Views.Plane> Planes { get; } = [];

    public bool HasNoPlanes => Planes.Count == 0;

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
        _mapper = (App.Current as App)!.Mapper;
        _mapper.PlaneAdded += Mapper_PlaneAdded;
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

    readonly List<string> _debugPlaneNames = [];

    readonly Services.Mapper _mapper;

    private void Mapper_PlaneAdded(object? sender, Models.Plane plane)
    {
        var planeView = new Views.Plane(plane);
        Planes.Add(planeView);

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasNoPlanes)));
    }
}