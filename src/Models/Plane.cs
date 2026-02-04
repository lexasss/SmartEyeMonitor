namespace SmartEyeMonitor.Models;

public class Plane
{
    public required string Name { get; init; }

    public bool IsEnabled { get; set; } = true;

    public bool IsFocused { get; set; } = false;

    public event EventHandler? Focused;
    public event EventHandler? Left;

    public virtual void Enter()
    {
        if (!IsEnabled)
            return;

        IsFocused = true;
        Focused?.Invoke(this, EventArgs.Empty);
    }

    public virtual void Exit()
    {
        if (!IsEnabled)
            return;

        IsFocused = false;
        Left?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Consumes the gaze point that fell onto a plane
    /// Reserved for future
    /// </summary>
    /// <param name="intersection">intersection data</param>
    public virtual void Feed(Intersection intersection)
    {
        if (!IsEnabled)
            return;
    }
}
