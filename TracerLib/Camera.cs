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
    /// The ratio Width/Height of the image.
    /// </summary>
    public float AspectRatio { get; }

    /// <summary>
    /// The transformation applied to the camera.
    /// </summary>
    public Transformation Transformation { get; set; }
    
    /// <summary>
    /// Returns a <see cref="Ray"/> passing through the specified screen's coordinates (u,v).
    /// The coordinate origin is the top-left corner of the screen.
    /// u and v are in [0, 1], mapped to screen space: y ∈ [-R, R] (R = aspect ratio) and z ∈ [-1, 1].
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
    /// Constructs an orthogonal <c>Camera</c> with specified aspect ratio and transformation.
    /// </summary>
    /// <param name="transformation">Possible <c>Transformation</c>: Identity, Translation, Scaling and Rotation around a specified axis.</param>
    /// <param name="aspectRatio">Ratio between image's width and height.</param>
    public OrthogonalCamera(Transformation transformation, float aspectRatio = 1.0f)
    {
        AspectRatio = aspectRatio;
        Transformation = transformation;
    }
    
    // forse sarebbe meglio mettere due parametri ulteriori da riga di comando che allargano la dimensione dello schermo.
    /// <summary>
    /// Returns a <see cref="Ray"/> orthogonal to the screen, passing through the specified screen's coordinates (u,v).
    /// The coordinate origin is the top-left corner of the screen.
    /// u and v are in [0, 1], mapped to screen space: y ∈ [-R, R] (R = aspect ratio) and z ∈ [-1, 1].
    /// </summary>
    /// <param name="u">Horizontal screen coordinate (within [0,1]).</param>
    /// <param name="v">Vertical screen coordinate (within [0,1]).</param>
    /// <returns></returns>
    public Ray FireRay(float u, float v)
    {
        // formula per quando u,v partono dall'angolo in basso a sinistra
        // u e v vanno da 0 a 1
        // e così lo schermo ha proporzioni 2x2*AspectRatio
        // il punto per cui passa il raggio va da -AspectRatio a +AspectRatio per y e da -1 a 1 per z 
        // Point origin = new Point(-1f, (1f - 2f * u) * this.AspectRatio, 2f * v - 1f);

        // formula per quando u e v partono dall'angolo in alto a sinistra
        // u e v vanno da 0 a 1
        // e così lo schermo ha proporzioni 2x2*AspectRatio
        // il punto per cui passa il raggio va da -AspectRatio a +AspectRatio per y e da -1 a 1 per z 
         Point origin = new Point(-1, (-2*u + 1f) * AspectRatio, -2*v + 1f);

        // formula per quando u e v partono dall'angolo in alto a sinistra
        // u e v vanno da 0 a 1
        // e così lo schermo ha proporzioni 1xAspectRatio 
        // il punto per cui passa il raggio va da -0.5*AspectRatio a 0.5*AspectRatio per y e da -0.5 a 0.5 per z
        // quindi forse così lo schermo è un po' più piccolo
        //Point origin = new Point(-1, (-u + 0.5f) * AspectRatio, -v + 0.5f);

        //direction orthogonal to the screen.
        Vector direction = new Vector(1, 0, 0);
        return Transformation * new Ray(origin, direction);
    }
}

/// <summary>
/// Represents a perspective camera projection.
/// In this model, all rays originate from a single point (the observer position),
/// creating a vanishing point effect where parallel lines converge in the distance.
/// Objects appear smaller as their distance from the camera increases, simulating human vision.
/// </summary>
public struct PerspectiveCamera : ICamera
{
    /// <summary>
    /// The distance of the observer from the screen.
    /// </summary>
    public float Distance { get; set; }

    public float AspectRatio { get; set; }
    public Transformation Transformation { get; set; }

    /// <summary>
    /// Initialize a perspective <c>Camera</c> with unity <see cref="Distance"/>,
    /// an <see cref="AspectRatio"/> of 1:1 and an identity <see cref="Transformation"/>.
    /// </summary>
    public PerspectiveCamera()
    {
        Distance = 1.0f;
        AspectRatio = 1.0f;
        Transformation = new Transformation();
    }

    /// <summary>
    /// Constructs a camera that uses a perspective projection,
    /// with specified <see cref="Distance"/>, <see cref="AspectRatio"/> and <see cref="Transformation"/>.
    /// </summary>
    /// <param name="transformation">Possible <c>Transformation</c>: Identity, Translation, Scaling and Rotation around a specified axis</param>
    /// <param name="distance">Distance between the observer and the screen</param>
    /// <param name="aspectRatio">Ratio Width/Height of the screen</param>
    public PerspectiveCamera(Transformation transformation, float distance = 1.0f, float aspectRatio = 1.0f)
    {
        Distance = distance;
        AspectRatio = aspectRatio;
        Transformation = transformation;
    }
    
    // forse sarebbe meglio mettere due parametri ulteriori da riga di comando che allargano la dimensione dello schermo.
    /// <summary>
    /// Returns a <see cref="Ray"/> starting at (-d, 0, 0)
    /// (d = <see cref="Distance"/> from between the observer and the screen)
    /// and passing through the specified screen's coordinates (u,v).
    /// The coordinate origin is the top-left corner of the screen.
    /// u and v are in [0, 1], mapped to screen space: y ∈ [-R, R] (R = aspect ratio) and z ∈ [-1, 1].
    /// </summary>
    /// <param name="u">Horizontal screen coordinate (within [0,1]).</param>
    /// <param name="v">Vertical screen coordinate (within [0,1]).</param>
    /// <returns></returns>
    public Ray FireRay(float u, float v)
    {
        // position of the observer
        Point origin = new Point(-Distance, 0.0f, 0.0f);

        // formula per quando u,v partono dall'angolo in basso a sinistra
        // u e v vanno da 0 a 1
        // e così lo schermo ha proporzioni 2x2*AspectRatio
        // il punto per cui passa il raggio va da -AspectRatio a +AspectRatio per y e da -1 a 1 per z 
        // Vector dir = new Vector(Distance, (1.0f - 2.0f * u) * AspectRatio, 2 * v - 1);

        // formula per quando u e v partono dall'angolo in alto a sinistra
        // u e v vanno da 0 a 1
        // e così lo schermo ha proporzioni 2x2*AspectRatio
        // il punto per cui passa il raggio va da -AspectRatio a +AspectRatio per y e da -1 a 1 per z
        Vector dir = new Vector(Distance, (-2*u + 1f) * AspectRatio, -2*v + 1f);

        // formula per quando u e v partono dall'angolo in alto a sinistra
        // u e v vanno da 0 a 1
        // e così lo schermo ha proporzioni 1xAspectRatio
        // il punto per cui passa il raggio va da -0.5*AspectRatio a 0.5*AspectRatio per y e da -0.5 a 0.5 per z
        // quindi forse così lo schermo è un po' più piccolo
        // Vector dir = new Vector(Distance, (-u + 0.5f) * AspectRatio, -v + 0.5f);

        return Transformation * new Ray(origin, dir);
    }
}