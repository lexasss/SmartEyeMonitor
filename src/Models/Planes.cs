namespace SmartEyeMonitor.Models;

/// <summary>
/// A collection SmartEye planes/screens.
/// Each plane is not aware of other planes.
/// </summary>
internal class Planes
{
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
    /// <param name="plane">plane/screen name</param>
    public void Enter(string plane)
    {
        if (!_planes.ContainsKey(plane))
        {
            if (Add(plane) == null)
                return;
        }

        _planes[plane].Enter();
    }

    /// <summary>
    /// Must be called for each gaze-off event
    /// </summary>
    /// <param name="plane">plane/screen name</param>
    public void Exit(string plane)
    {
        if (!_planes.ContainsKey(plane))
        {
            if (Add(plane) == null)
                return;
        }

        _planes[plane].Exit();
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
