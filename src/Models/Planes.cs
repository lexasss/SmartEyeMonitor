namespace SmartEyeMonitor.Models;

/// <summary>
/// A collection SmartEye planes/screens.
/// Each plane is not aware of other planes.
/// </summary>
internal class Planes
{
    /// <summary>
    /// Fires when a new plane is generated based on received gaze data
    /// </summary>
    public event EventHandler<Plane>? PlaneAdded;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="panels">UI panels that represent planes/screen</param>
    public Planes(params string[] planeNames) : this((IEnumerable<string>)planeNames) { }


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="panels">UI panels that represent planes/screen</param>
    public Planes(IEnumerable<string> planeNames)
    {
        foreach (var name in planeNames)
        {
            _planes.Add(name, new Plane() { Name = name });
        }
    }

    public Plane? Add(string name)
    {
        Plane? plane = null;

        if (!string.IsNullOrEmpty(name))
        {
            plane = new Plane() { Name = name };
            _planes.Add(name, plane);
        }

        return plane;
    }

    /// <summary>
    /// Must be called for each gaze-on event
    /// </summary>
    /// <param name="planeName">plane/screen name</param>
    public void Enter(string planeName)
    {
        if (!_planes.ContainsKey(planeName))
        {
            var plane = Add(planeName);
            if (plane == null)
                return;

            PlaneAdded?.Invoke(this, plane);
        }

        _planes[planeName].Enter();
    }

    /// <summary>
    /// Must be called for each gaze-off event
    /// </summary>
    /// <param name="planeName">plane/screen name</param>
    public void Exit(string planeName)
    {
        if (!_planes.ContainsKey(planeName))
        {
            var plane = Add(planeName);
            if (plane == null)
                return;

            PlaneAdded?.Invoke(this, plane);
        }

        _planes[planeName].Exit();
    }

    /// <summary>
    /// Consumes a gaze sample
    /// Reserved for future, as this method is never called
    /// </summary>
    /// <param name="intersections">intersection with planes</param>
    public void Feed(List<Intersection> intersections)
    {
        foreach (var intersection in intersections)
        {
            if (_planes.TryGetValue(intersection.PlaneName, out var plane))
            {
                plane.Feed(intersection);
            }
        }
    }

    // Internal

    readonly Dictionary<string, Plane> _planes = new();
}
