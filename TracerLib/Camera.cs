namespace TracerLib;

public interface ICamera
{
    public float AspectRatio { get; set; }
    public Ray FireRay(float u, float v);
}

public struct OrthogonalCamera : ICamera
{
    public float AspectRatio { get; set; }
    public Transformation Transformation { get; set; }

    public OrthogonalCamera(float aspectRatio = 1.0f, Transformation transformation = default)
    {
        AspectRatio = aspectRatio;
        Transformation = transformation;
    }

    public Ray FireRay(float u, float v)
    {
        var origin = new Point(-1f, (1f - 2f * u) * this.AspectRatio, 2f * v - 1f);
        var direction = new Vector(1, 0, 0);
        return this.Transformation * new Ray(origin, direction);
    }
}

public struct PerspectiveCamera : ICamera
{
    public float Distance { get; set; }
    public float AspectRatio { get; set; }
    public Transformation Transformation { get; set; }

    public PerspectiveCamera(float distance = 1.0f, float aspectRatio = 1.0f, Transformation transformation = default)
    {
        Distance = distance;
        AspectRatio = aspectRatio;
        Transformation = transformation;
    }

    /// <summary>
    /// Returns a <c>Ray</c> starting at (-d, 0, 0) and directed toward the point defined by normalized screen coordinates (u, v).
    /// u and v are in [0, 1], mapped to screen space: x ∈ [-R, R] (R = aspect ratio) and y ∈ [-1, 1].
    /// </summary>
    /// <param name="u"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public Ray FireRay(float u, float v)
    {
        var origin = new Point(-this.Distance, 0.0f, 0.0f);
        var dir = new Vector(this.Distance, (1.0f - 2.0f * u) * this.AspectRatio, 2 * v - 1);
        return this.Transformation * new Ray(origin, dir);
    }
}