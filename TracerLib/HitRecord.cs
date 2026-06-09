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
    /// SurfaceNormal is the surface normal at the intersection point,
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

    public HitRecord(Point worldPoint, Shape shape, Normal normal, Vector2D surfacePoint, Ray ray, float t)
    {
        this.WorldPoint = worldPoint;
        this.Shape = shape;
        this.SurfaceNormal = normal;
        this.SurfacePosition = surfacePoint;
        this.IncomingRay = ray;
        this.T = t;
    }

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