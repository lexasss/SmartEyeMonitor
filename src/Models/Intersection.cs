namespace SmartEyeMonitor.Models;

public struct Intersection
{
    public int ID;
    public string PlaneName;
    public SEClient.Tcp.Point3D Gaze;
    public SEClient.Tcp.Point2D Point;
}