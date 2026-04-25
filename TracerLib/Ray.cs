namespace TracerLib;

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

    public static bool _AreRaysClose(Ray r1, Ray r2)
    {
        return Vector._AreVectorsClose(r1.Dir, r2.Dir) && Point._ArePointsClose(r1.Origin, r2.Origin);
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