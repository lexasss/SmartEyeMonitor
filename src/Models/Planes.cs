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
    /// <param name="planeName">plane/screen name</param>
    /// <param name="plane">plane object</param>
    /// <returns>True if the plane was added, false otherwise</returns>
    public bool Enter(string planeName, out Plane? plane)
    {
        bool wasAdded = false;
        plane = null;

        if (!_planes.ContainsKey(planeName))
        {
            wasAdded = Add(planeName) != null;
            if (!wasAdded)
                return false;
        }

        plane = _planes[planeName];
        plane.Enter();

        return wasAdded;
    }

    /// <summary>
    /// Must be called for each gaze-off event
    /// </summary>
    /// <param name="planeName">plane/screen name</param>
    /// <param name="plane">plane object</param>
    /// <returns>True if the plane was added, false otherwise</returns>
    public bool Exit(string planeName, out Plane? plane)
    {
        bool wasAdded = false;
        plane = null;

        if (!_planes.ContainsKey(planeName))
        {
            wasAdded = Add(planeName) != null;
            if (!wasAdded)
                return false;
        }

        plane = _planes[planeName];
        plane.Exit();

        return wasAdded;
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
