// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// A struct representing a ray.
/// 
/// Origin is the point where the ray is generated.
/// Dir is the direction at which the ray points to.
/// Tmin and Tmax are parameters used to compute intersections.
/// If an intersection occurs at the point P = Origin + t * Dir then t must satisfy the inequality Tmin <= t <= Tmax
/// Depth counts how many times the ray has been already reflected.
/// </summary>
public struct Ray
{
    public Point Origin { get; set; }
    public Vector Dir { get; set; }
    public float Tmin { get; private set; }
    public float Tmax { get; private set; }
    public int Depth { get; private set; }

    public Ray(Point origin, Vector dir, float tmin = 1e-5f, float tmax = float.PositiveInfinity, int depth = 0)
    {
        Origin = origin;
        Dir = dir;
        Tmin = tmin;
        Tmax = tmax;
        Depth = depth;
    }

    public static bool _AreRaysClose(Ray r1, Ray r2, float epsilon = 1e-5f)
    {
        return Vector._AreVectorsClose(r1.Dir, r2.Dir, epsilon) && Point._ArePointsClose(r1.Origin, r2.Origin, epsilon);
    }

    public override string ToString()
    {
        return "Ray(Origin=" + Origin + ", Dir=" + Dir + ", Tmin=" + Tmin + ", Tmax=" + Tmax + ", Depth=" + Depth + ")";
    }

    public void Print()
    {
        Console.WriteLine(ToString());
    }

    public Point At(float t)
    {
        return this.Origin + this.Dir * t;
    }

    public static Ray operator *(Transformation t, Ray r)
    {
        return new Ray(t * r.Origin, t * r.Dir);
    }
}