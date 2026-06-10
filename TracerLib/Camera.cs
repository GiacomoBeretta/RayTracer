// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// Defines the common contract for a camera used to project a scene onto an image.
/// The camera screen is parametrized as the unit square [0,1] × [0,1], with the origin in the top-left corner.
/// The <see cref="AspectRatio"/> property can then be used to scale it to the desired proportions.
/// </summary>
public interface ICamera
{
    /// <summary>
    /// The ratio between the width and the height of the image.
    /// </summary>
    public float AspectRatio { get; }

    /// <summary>
    /// The transformation applied to the camera.
    /// </summary>
    public Transformation Transformation { get; set; }

    /// <summary>
    /// Generates a <see cref="Ray"/> for the specified screen's coordinates.
    /// The coordinate origin is the top-left corner of the screen.
    /// u,v are in [0,1].
    /// </summary>
    /// <param name="u">Horizontal screen coordinate (within [0,1]).</param>
    /// <param name="v">Vertical screen coordinate (within [0,1]).</param>
    /// <returns></returns>
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

    public Transformation Transformation { get; set; }

    /// <summary>
    /// Constructs an <see cref="OrthogonalCamera"/> with an aspect ratio of 1:1 and an identity <c>Transformation</c>
    /// </summary>
    public OrthogonalCamera()
    {
        AspectRatio = 1.0f;
        Transformation = new Transformation();
    }

    /// <summary>
    /// Constructs an orthogonal <c>Camera</c> with specified aspect ratio and <c>Transformation</c>.
    /// </summary>
    /// <param name="transformation">Possible <c>Transformation</c>: Identity, Translation, Scaling and Rotation around a specified axis</param>
    /// <param name="aspectRatio">Ratio between image's width and height</param>
    public OrthogonalCamera(Transformation transformation, float aspectRatio = 1.0f)
    {
        AspectRatio = aspectRatio;
        Transformation = transformation;
    }
   
    /// <summary>
    /// Generates a <see cref="Ray"/> for the specified screen's coordinates.
    /// The coordinate origin is the top-left corner of the screen.
    /// u,v are in [0,1].
    /// </summary>
    /// <param name="u">Horizontal screen coordinate (within [0,1]).</param>
    /// <param name="v">Vertical screen coordinate (within [0,1]).</param>
    /// <returns></returns>
    public Ray FireRay(float u, float v)
    {
        var origin = new Point(-1f, (1f - 2f * u) * this.AspectRatio, 2f * v - 1f);
        var direction = new Vector(1, 0, 0);
        return this.Transformation * new Ray(origin, direction);
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
    /// Initialize a perspective <c>Camera</c> with unity distance, an aspect ratio of 1:1 and an identity <c>Transformation</c>
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