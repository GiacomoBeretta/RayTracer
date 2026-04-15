namespace TracerLib;

public struct Ray
{
    public Point Origin { get; set; }
    public Vector Dir { get; set; }
    public float Tmin { get; private set; } = 1e-5f;
    public float Tmax { get; private set; } = float.PositiveInfinity;
    public int Depth { get; private set; } = 0;

    public Ray(Point origin, Vector dir, float tmin, float tmax, int depth)
    {
        Origin = origin;
        Dir = dir;
        Tmin = tmin;
        Tmax = tmax;
        Depth = depth;
    }

    public static bool _AreRayClose(Ray r1, Ray r2, float epsilon = 1e-5f)
    {
        return Vector._AreVectorsClose(r1.Dir, r2.Dir) && Point._ArePointClose(r1.Origin, r2.Origin);  
    }

    public Point At(float t)
    {
        return this.Origin + this.Dir * t; 
    }
    
}