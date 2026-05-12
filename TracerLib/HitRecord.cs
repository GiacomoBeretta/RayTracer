// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

public struct HitRecord
{
    public Point WorldPoint { get; }

    public Shape Shape { get; }
    public Normal SurfaceNormal { get; }
    public Vector2D SurfacePoint { get; }
    public Ray IncomingRay { get; }
    public float T { get; }

    public HitRecord(Point worldPoint, Shape shape, Normal normal, Vector2D surfacePoint, Ray ray, float t)
    {
        this.WorldPoint = worldPoint;
        this.Shape = shape;
        this.SurfaceNormal = normal;
        this.SurfacePoint = surfacePoint;
        this.IncomingRay = ray;
        this.T = t;
    }

    public static bool _AreHitRecordsClose(HitRecord hit1, HitRecord hit2, float epsilon = 1e-5f)
    {
        return Point._ArePointsClose(hit1.WorldPoint, hit2.WorldPoint, epsilon)
               && (hit1.Shape)._IsCloseTo(hit2.Shape, epsilon)
               && Normal._AreNormalsClose(hit1.SurfaceNormal, hit2.SurfaceNormal, epsilon)
               && Vector2D._AreVectorsClose(hit1.SurfacePoint, hit2.SurfacePoint, epsilon)
               && Ray._AreRaysClose(hit1.IncomingRay, hit2.IncomingRay, epsilon)
               && Functions.AreClose(hit1.T, hit2.T, epsilon);
    }

    public override string ToString()
    {
        return "HitRecord(WorldPoint=" + WorldPoint +
               ", Shape=" + Shape +
               ", SurfaceNormal=" + SurfaceNormal +
               ", SurfacePoint=" + SurfacePoint +
               ", IncomingRay=" + IncomingRay +
               ", T=" + T;
    }

    public void Print()
    {
        Console.WriteLine(ToString());
    }
}