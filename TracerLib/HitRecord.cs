// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// A struct representing the intersection between a shape and a ray.
/// </summary>
public struct HitRecord
{
    /// <summary>
    /// The point where the intersection occurs.
    /// </summary>
    public Point WorldPoint { get; }

    /// <summary>
    /// The shape intersected by the IncomingRay.
    /// </summary>
    public Shape Shape { get; }

    /// <summary>
    /// SurfaceNormal is the normal to the surface at the intersection point,
    /// typically oriented against the incoming ray direction.
    /// </summary>
    public Normal SurfaceNormal { get; }

    /// <summary>
    /// The 2D surface coordinates of the intersection point WorldPoint.
    /// </summary>
    public Vector2D SurfacePosition { get; }

    /// <summary>
    /// The ray that intersects the shape.
    /// </summary>
    public Ray IncomingRay { get; }

    /// <summary>
    /// T is the parameter such that WorldPoint = IncomingRay.Origin + T * IncomingRay.Dir
    /// </summary>
    public float T { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="HitRecord"/> struct,
    /// representing a geometric intersection between a ray and a shape.
    /// </summary>
    /// <param name="worldPoint">
    /// The point where the ray intersects the shape.
    /// </param>
    /// <param name="shape">
    /// The shape that was intersected by the ray.
    /// </param>
    /// <param name="normal">
    /// The normal to the surface at the intersection point, typically oriented
    /// against the incoming ray direction.
    /// </param>
    /// <param name="surfacePoint">
    /// The 2D parametric coordinates of the intersection point on the surface.
    /// </param>
    /// <param name="ray">
    /// The incoming ray that produced this intersection.
    /// </param>
    /// <param name="t">
    /// The ray parameter at the intersection point, such that:
    /// worldPoint = ray.Origin + t * ray.Direction.
    /// </param>
    public HitRecord(Point worldPoint, Shape shape, Normal normal, Vector2D surfacePoint, Ray ray, float t)
    {
        WorldPoint = worldPoint;
        Shape = shape;
        SurfaceNormal = normal;
        SurfacePosition = surfacePoint;
        IncomingRay = ray;
        T = t;
    }

    /// <summary>
    /// Determines whether two <see cref="HitRecord"/> instances represent
    /// the same geometric intersection within a given tolerance.
    /// </summary>
    /// <param name="hit1">The first hit record to compare.</param>
    /// <param name="hit2">The second hit record to compare.</param>
    /// <param name="epsilon">
    /// The tolerance used for approximate equality checks across all fields.
    /// </param>
    /// <returns>
    /// True if all components of the two hit records (world position, shape,
    /// surface normal, UV coordinates, incoming ray, and parameter t)
    /// are equal within the specified tolerance; otherwise false.
    /// </returns>
    /// <remarks>
    /// This method performs approximate comparisons to account for floating-point
    /// precision errors. It is intended for testing and geometric validation rather
    /// than strict identity checks.
    /// </remarks>
    public static bool _AreHitRecordsClose(HitRecord hit1, HitRecord hit2, float epsilon = 1e-5f)
    {
        return Point._ArePointsClose(hit1.WorldPoint, hit2.WorldPoint, epsilon)
               && (hit1.Shape)._IsCloseTo(hit2.Shape, epsilon)
               && Normal._AreNormalsClose(hit1.SurfaceNormal, hit2.SurfaceNormal, epsilon)
               && Vector2D._AreVectorsClose(hit1.SurfacePosition, hit2.SurfacePosition, epsilon)
               && Ray._AreRaysClose(hit1.IncomingRay, hit2.IncomingRay, epsilon)
               && Functions.AreClose(hit1.T, hit2.T, epsilon);
    }

    public override string ToString()
    {
        return "HitRecord(WorldPoint=" + WorldPoint +
               ", Shape=" + Shape +
               ", SurfaceNormal=" + SurfaceNormal +
               ", SurfacePoint=" + SurfacePosition +
               ", IncomingRay=" + IncomingRay +
               ", T=" + T;
    }

    public void Print()
    {
        Console.WriteLine(ToString());
    }
}