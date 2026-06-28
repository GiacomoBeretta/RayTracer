// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// Defines the common contract for a camera used to project a scene onto an image.
/// </summary>
/// <remarks>
/// The camera uses the following right-handed coordinate system:
/// X: forward (viewing direction)
/// Y: horizontal axis, positive toward the left
/// Z: vertical axis, positive upward
/// 
/// The image plane is centered at the origin of the coordinate system.
/// It is parametrized using normalized coordinates (u, v) in [0, 1] × [0, 1],
/// with (0,0) in the top-left corner.
/// (u,v) are mapped by the method <see cref="FireRay"/>
/// onto the image plane [-Width/2, Width/2] x [-Height/2,Height/2]
/// </remarks>
public interface ICamera
{
    /// <summary>
    /// Width of the image plane.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Height of the image plane.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// The ratio Width/Height of the image plane.
    /// </summary>
    public float AspectRatio => Width / Height;

    /// <summary>
    /// The transformation applied to the camera.
    /// </summary>
    public Transformation Transformation { get; set; }

    /// <summary>
    /// Returns a <see cref="Ray"/> that passes through the specified normalized image coordinates (u, v).
    /// </summary>
    /// <remarks>
    /// (u, v) parameterize the image plane in normalized coordinates in the range [0, 1] × [0, 1],
    /// with (0, 0) mapped to the top-left corner.
    ///
    /// These coordinates are transformed into image plane space:
    /// y ∈ [-Width/2, Width/2] (horizontal axis),
    /// z ∈ [-Height/2, Height/2] (vertical axis).
    /// 
    /// See <see cref="ICamera"/> for details about the coordinate system.
    /// </remarks>
    /// <param name="u">Horizontal normalized coordinate in the range [0, 1].</param>
    /// <param name="v">Vertical normalized coordinate in the range [0, 1].</param>
    /// <returns></returns>
    public Ray FireRay(float u, float v);
}

/// <summary>
/// Represents an orthogonal (orthographic) camera projection.
/// In this projection, all rays are parallel to each other and perpendicular to the image plane,
/// meaning there is no perspective distortion.
///
/// See <see cref="ICamera"/> for more information.
/// </summary>
public struct OrthogonalCamera : ICamera
{
    public float Width { get; }
    public float Height { get; }
    public float AspectRatio { get; }
    public Transformation Transformation { get; set; }

    /// <summary>
    /// Constructs an <see cref="OrthogonalCamera"/> with a 1×1 image plane and the
    /// identity <see cref="Transformation"/>.
    /// </summary>
    public OrthogonalCamera()
    {
        Width = 1.0f;
        Height = 1.0f;
        Transformation = new Transformation();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrthogonalCamera"/> class with the specified
    /// transformation and image plane dimensions.
    /// </summary>
    /// <param name="transformation">
    /// The transformation that defines the camera's position and orientation.
    /// </param>
    /// <param name="width">The width of the image plane (defaults to 1).</param>
    /// <param name="height">The height of the image plane (defaults to 1).</param>
    public OrthogonalCamera(Transformation transformation, float width = 1.0f, float height = 1.0f)
    {
        Width = width;
        Height = height;
        Transformation = transformation;
    }

    /// <summary>
    /// Returns a <see cref="Ray"/> that passes through the specified normalized image coordinates (u, v),
    /// parallel to the camera's viewing direction (X axis).
    /// </summary>
    /// <remarks>
    /// (u, v) parameterize the image plane in normalized coordinates in the range [0, 1] × [0, 1],
    /// with (0, 0) mapped to the top-left corner.
    ///
    /// These coordinates are transformed into image plane space:
    /// y ∈ [-Width/2, Width/2] (horizontal axis),
    /// z ∈ [-Height/2, Height/2] (vertical axis).
    /// 
    /// See <see cref="ICamera"/> for details about the coordinate system.
    /// </remarks>
    /// <param name="u">Horizontal normalized coordinate in the range [0, 1].</param>
    /// <param name="v">Vertical normalized coordinate in the range [0, 1].</param>
    /// <returns></returns>
    public Ray FireRay(float u, float v)
    {
        // Map normalized coordinates [0,1] to centered image plane coordinates [-0.5, 0.5].
        // We don't compute (u - 0.5, v - 0.5) because
        // u,v originate in the top-left corner and have the positive direction inverted with respect to y and z.
        // We multiply by Width and Height to scale the normalized image plane to its real dimensions.
        Point origin = new Point(-1, (-u + 0.5f) * Width, (-v + 0.5f) * Height);

        // In orthogonal projection, all rays are parallel to the camera's viewing direction (X axis),
        // which is orthogonal to the image plane.
        Vector direction = new Vector(1, 0, 0);
        return Transformation * new Ray(origin, direction);
    }
}

/// <summary>
/// Represents a perspective camera projection.
/// In this model, all rays originate from a single point (the observer position),
/// creating a vanishing point effect where parallel lines converge in the distance.
/// Objects appear smaller as their distance from the camera increases, simulating human vision.
///
/// See <see cref="ICamera"/> for more information.
/// </summary>
public struct PerspectiveCamera : ICamera
{
    public float Width { get; }
    public float Height { get; }
    public float AspectRatio { get; set; }

    /// <summary>
    /// The distance of the observer from the screen.
    /// </summary>
    public float Distance { get; set; }

    public Transformation Transformation { get; set; }

    /// <summary>
    /// Constructs a <see cref="PerspectiveCamera"/> with a 1×1 image plane,
    /// the identity <see cref="Transformation"/>, and a unit <see cref="Distance"/> to the image plane.
    /// </summary>
    public PerspectiveCamera()
    {
        Distance = 1.0f;
        Width = 1.0f;
        Height = 1.0f;
        Transformation = new Transformation();
    }

    /// <summary>
    /// Constructs a <see cref="PerspectiveCamera"/>, with the specified image plane dimensions, <see cref="Distance"/> to the image plane,
    /// and <see cref="Transformation"/>.
    /// </summary>
    /// <param name="transformation">The transformation that defines the camera's position and orientation.</param>
    /// <param name="width">The width of the image plane (defaults to 1).</param>
    /// <param name="height">The height of the image plane (defaults to 1).</param>
    /// <param name="distance">Distance to the image plane (defaults to 1).</param>
    public PerspectiveCamera(Transformation transformation, float width = 1.0f, float height = 1.0f,
        float distance = 1.0f)
    {
        Width = width;
        Height = height;
        Distance = distance;
        Transformation = transformation;
    }

    /// <summary>
    /// Returns a <see cref="Ray"/> starting at (-d, 0, 0)
    /// (d = <see cref="Distance"/> from between the observer and the screen)
    /// that passes through the specified normalized image coordinates (u, v),
    /// </summary>
    /// <remarks>
    /// (u, v) parameterize the image plane in normalized coordinates in the range [0, 1] × [0, 1],
    /// with (0, 0) mapped to the top-left corner.
    ///
    /// These coordinates are transformed into image plane space:
    /// y ∈ [-Width/2, Width/2] (horizontal axis),
    /// z ∈ [-Height/2, Height/2] (vertical axis).
    /// 
    /// See <see cref="ICamera"/> for details about the coordinate system.
    /// </remarks>
    /// <param name="u">Horizontal normalized coordinate in the range [0, 1].</param>
    /// <param name="v">Vertical normalized coordinate in the range [0, 1].</param>
    /// <returns>A ray originating from the camera and passing through the corresponding point on the image plane.</returns>
    public Ray FireRay(float u, float v)
    {
        // position of the observer
        Point origin = new Point(-Distance, 0.0f, 0.0f);

        // Map normalized coordinates [0,1] to centered image plane coordinates [-0.5, 0.5].
        // We don't compute (u - 0.5, v - 0.5) because
        // u,v originate in the top-left corner and have the positive direction inverted with respect to y and z.
        // We multiply by Width and Height to scale the normalized image plane to its real dimensions.
        Vector dir = new Vector(Distance, (-u + 0.5f) * Width, (-v + 0.5f) * Height);

        return Transformation * new Ray(origin, dir);
    }
}