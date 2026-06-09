// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

public interface ICamera
{
    public float AspectRatio { get; set; }
    public Ray FireRay(float u, float v);
}

/// <summary>
/// Represents an orthogonal (orthographic) camera projection.
/// In this projection, all rays are parallel to each other and perpendicular to the image plane,
/// meaning there is no perspective distortion.
/// </summary>
public struct OrthogonalCamera : ICamera
{
    public float AspectRatio { get; set; }
    public Transformation Transform { get; set; }

    /// <summary>
    /// Initialize an orthogonal <c>Camera</c> with unity aspect ratio and identity <c>Transformation</c>
    /// </summary>
    public OrthogonalCamera()
    {
        AspectRatio = 1.0f;
        Transform = new Transformation();
    }

    /// <summary>
    /// Initialize an orthogonal <c>Camera</c> with specified aspect ratio and <c>Transformation</c>
    /// </summary>
    /// <param name="transformation">Possible <c>Transformation</c>: Identity, Translation, Scaling and Rotation around a specified axis</param>
    /// <param name="aspectRatio">Ratio between image's width and height</param>
    public OrthogonalCamera(Transformation transformation, float aspectRatio = 1.0f)
    {
        AspectRatio = aspectRatio;
        Transform = transformation;
    }

    /// <summary>
    /// Generate a <c>Ray</c> for the specified screen's coordinates
    /// </summary>
    /// <param name="u">Horizontal screen coordinate</param>
    /// <param name="v">Vertical screen coordinate</param>
    /// <returns></returns>
    public Ray FireRay(float u, float v)
    {
        var origin = new Point(-1f, (1f - 2f * u) * this.AspectRatio, 2f * v - 1f);
        var direction = new Vector(1, 0, 0);
        return this.Transform * new Ray(origin, direction);
    }
}

/// <summary>
/// Represents a perspective camera projection.
/// In this model, all rays originate from a single point (the camera position),
/// creating a vanishing point effect where parallel lines converge in the distance.
/// Objects appear smaller as their distance from the camera increases, simulating human vision.
/// </summary>
public struct PerspectiveCamera : ICamera
{
    public float Distance { get; set; }
    public float AspectRatio { get; set; }
    public Transformation Transformation { get; set; }

    /// <summary>
    /// Initialize a perspective <c>Camera</c> with unity distance, aspect ratio and identity <c>Transformation</c>
    /// </summary>
    public PerspectiveCamera()
    {
        Distance = 1.0f;
        AspectRatio = 1.0f;
        Transformation = new Transformation();
    }

    /// <summary>
    /// Initialize a perspective <c>Camera</c> with specified distance, aspect ratio and <c>Transformation</c>
    /// </summary>
    /// <param name="transformation">Possible <c>Transformation</c>: Identity, Translation, Scaling and Rotation around a specified axis</param>
    /// <param name="distance">Distance of the camera with  respect of the axis's origin on the x-axis</param>
    /// <param name="aspectRatio">Ratio between image's width and height</param>
    public PerspectiveCamera(Transformation transformation, float distance = 1.0f, float aspectRatio = 1.0f)
    {
        Distance = distance;
        AspectRatio = aspectRatio;
        Transformation = transformation;
    }

    /// <summary>
    /// Returns a <c>Ray</c> starting at (-d, 0, 0) and directed toward the point defined by normalized screen coordinates (u, v).
    /// u and v are in [0, 1], mapped to screen space: x ∈ [-R, R] (R = aspect ratio) and y ∈ [-1, 1].
    /// </summary>
    /// <param name="u">Horizontal screen coordinate </param>
    /// <param name="v">Vertical screen coordinate</param>
    /// <returns></returns>
    public Ray FireRay(float u, float v)
    {
        var origin = new Point(-this.Distance, 0.0f, 0.0f);
        var dir = new Vector(this.Distance, (1.0f - 2.0f * u) * this.AspectRatio, 2 * v - 1);
        return this.Transformation * new Ray(origin, dir);
    }
}