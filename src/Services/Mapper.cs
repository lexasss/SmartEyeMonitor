using SmartEyeMonitor.Converters;
using System.ComponentModel;

namespace SmartEyeMonitor.Services;

[TypeConverter(typeof(FriendlyEnumConverter))]
public enum MappingMode
{
    AllPlanes,
    ClosestPlane
}

public class Mapper
{
    public MappingMode Mode { get; set; } = MappingMode.AllPlanes;

    public event EventHandler<Models.Plane>? PlaneAdded;


    public Mapper(string[]? planeNames = null)
    {
        _planes = new Models.Planes(planeNames ?? []);
    }

    public Models.Plane? Add(string name) => _planes.Add(name);

    public void Feed(SmartEyeTools.Data.Sample sample)
    {
        HandleIntersection(sample);
    }

    public void Reset()
    {
        HandleIntersection(new SmartEyeTools.Data.Sample());
    }

    // Internal

    readonly Models.Planes _planes;

    string _currentIntersectionName = "";
    HashSet<string> _currentIntersectionNames = new();

    private void HandleIntersection(SmartEyeTools.Data.Sample sample)
    {
        if (Mode == MappingMode.AllPlanes)
            HandleAllIntersections(sample);
        else
            HandleClosestIntersection(sample);
    }

    private void HandleClosestIntersection(SmartEyeTools.Data.Sample sample)
    {
        var seClientOptions = SmartEyeTools.Options.Instance;
        var intersectionSource = (seClientOptions.IntersectionSource, seClientOptions.IntersectionSourceFiltered) switch
        {
            (SmartEyeTools.IntersectionSource.Gaze, false) => sample.ClosestWorldIntersection,
            (SmartEyeTools.IntersectionSource.Gaze, true) => sample.FilteredClosestWorldIntersection,
            (SmartEyeTools.IntersectionSource.AI, false) => sample.EstimatedClosestWorldIntersection,
            (SmartEyeTools.IntersectionSource.AI, true) => sample.FilteredEstimatedClosestWorldIntersection,
            _ => throw new Exception($"This intersection source is not implemented")
        };

        if (intersectionSource is SmartEyeTools.WorldIntersection intersection)
        {
            var intersectionName = intersection.ObjectName.AsString;
            if (_currentIntersectionName != intersectionName)
            {
                _currentIntersectionName = intersectionName;
                HandleNewPlane(_planes.Enter(intersectionName, out var plane), plane);
            }
        }
        else if (!string.IsNullOrEmpty(_currentIntersectionName))
        {
            HandleNewPlane(_planes.Exit(_currentIntersectionName, out var plane), plane);
            _currentIntersectionName = "";
        }
    }

    private void HandleAllIntersections(SmartEyeTools.Data.Sample sample)
    {
        var seClientOptions = SmartEyeTools.Options.Instance;
        var intersectionSources = (seClientOptions.IntersectionSource, seClientOptions.IntersectionSourceFiltered) switch
        {
            (SmartEyeTools.IntersectionSource.Gaze, false) => sample.AllWorldIntersections,
            (SmartEyeTools.IntersectionSource.Gaze, true) => sample.FilteredAllWorldIntersections,
            (SmartEyeTools.IntersectionSource.AI, false) => sample.EstimatedAllWorldIntersections,
            (SmartEyeTools.IntersectionSource.AI, true) => sample.FilteredEstimatedAllWorldIntersections,
            _ => throw new Exception($"This intersection source is not implemented")
        };

        var activePlanes = new HashSet<string>();
        if (intersectionSources is SmartEyeTools.WorldIntersection[] intersections)
        {
            foreach (var intersection in intersections)
            {
                var intersectionName = intersection.ObjectName.AsString;
                activePlanes.Add(intersectionName);

                if (!_currentIntersectionNames.Contains(intersectionName))
                {
                    HandleNewPlane(_planes.Enter(intersectionName, out var plane), plane);
                }
            }
        }

        _currentIntersectionNames.ExceptWith(activePlanes);
        foreach (var intersectionName in _currentIntersectionNames)
        {
            HandleNewPlane(_planes.Exit(intersectionName, out var plane), plane);
        }

        _currentIntersectionNames = activePlanes;
    }

    private void HandleNewPlane(bool wasAdded, Models.Plane? plane)
    {
        if (wasAdded && plane != null)
        {
            PlaneAdded?.Invoke(this, plane);
        }
    }
}
