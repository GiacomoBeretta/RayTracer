// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// A struct representing a light ray.
/// 
/// Intersections are parameterized by a scalar t such that
/// P = <see cref="Origin"/> + t * <see cref="Dir"/>.
/// A valid intersection must satisfy:
/// <see cref="Tmin"/>  &lt;= t  &lt;= <see cref="Tmax"/>.
/// 
/// The Depth property stores the number of reflections
/// undergone by the ray.
/// </summary>
public struct Ray
{
    /// <summary>
    /// Origin is the origin of the <see cref="Dir"/> vector.
    /// </summary>
    public Point Origin { get; set; }

    /// <summary>
    /// Dir is the direction at which the ray points to.
    /// </summary>
    public Vector Dir { get; set; }

    /// <summary>
    /// Lower bound for valid intersection parameters.
    /// </summary>
    public float Tmin { get; private set; }

    /// <summary>
    /// Upper bound for valid intersection parameters.
    /// </summary>
    public float Tmax { get; private set; }

    /// <summary>
    /// Depth counts how many times the ray has been already reflected.
    /// </summary>
    public int Depth { get; private set; }

    /// <summary>
    /// Constructs an invalid/default ray whose origin is at (0, 0, 0),
    /// whose direction is the zero vector, a intersection range of [1e-5, +∞),
    /// and a recursion depth, i.e. the number of reflections, of 0.
    /// </summary>
    public Ray()
    {
        Origin = new Point(0, 0, 0);
        Dir = new Vector();
        Tmin = 1e-5f;
        Tmax = float.PositiveInfinity;
        Depth = 0;
    }

    public Ray(Point origin, Vector dir, float tmin = 1e-5f, float tmax = float.PositiveInfinity, int depth = 0)
    {
        Origin = origin;
        Dir = dir;
        Tmin = tmin;
        Tmax = tmax;
        Depth = depth;
    }

    /// <summary>
    /// Returns whether the two rays are approximately equal within a given tolerance.
    /// </summary>
    /// <remarks>
    /// The comparison uses <paramref name="epsilon"/> for the origin and direction
    /// components, while Tmin, Tmax, and Depth must match exactly.
    /// </remarks>
    /// <param name="r1">The first ray to compare.</param>
    /// <param name="r2">The second ray to compare.</param>
    /// <param name="epsilon">The tolerance used when comparing the origin and direction components.
    /// Defaults to 1e-5.</param>
    /// <returns>
    /// <see langword="true"/> if the rays have approximately equal origins and
    /// directions and the same recursion depth; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool _AreRaysClose(Ray r1, Ray r2, float epsilon = 1e-5f)
    {
        return Vector._AreVectorsClose(r1.Dir, r2.Dir, epsilon)
               && Point._ArePointsClose(r1.Origin, r2.Origin, epsilon)
               && r1.Tmin == r2.Tmin
               && r1.Tmax == r2.Tmax
               && r1.Depth == r2.Depth;
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
        return Origin + Dir * t;
    }

    public static Ray operator *(Transformation t, Ray r)
    {
        return new Ray(t * r.Origin, t * r.Dir);
    }
}