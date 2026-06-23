// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// Represents a point in 3D space.
/// </summary>
public struct Point
{
    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    public Point()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }

    public Point(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public override string ToString()
    {
        string point = $"Point(x={X}, y={Y}, z={Z})";
        return point;
    }

    public void Print()
    {
        Console.WriteLine(ToString());
    }

    /// <summary>
    /// Determines whether the two points are approximately equal within a given tolerance.
    /// </summary>
    /// <param name="a">The first point to compare.</param>
    /// <param name="b">The second point to compare.</param>
    /// <param name="epsilon">
    /// Tolerance threshold used for floating-point comparison of each component. Defaults to 1e-5.
    /// </param>
    /// <returns>True if all corresponding components of the two points differ by no more than epsilon.</returns>
    public static bool _ArePointsClose(Point a, Point b, float epsilon = 1e-5f)
    {
        return Functions.AreClose(a.X, b.X, epsilon)
               && Functions.AreClose(a.Y, b.Y, epsilon)
               && Functions.AreClose(a.Z, b.Z, epsilon);
    }

    /// <summary>
    /// Translates a point by a vector, returning a new resulting point in 3D space.
    /// </summary>
    /// <param name="p">The original point.</param>
    /// <param name="v">The vector to add.</param>
    /// <returns>A new <see cref="Point"/> obtained by applying the vector translation to the input point.</returns>
    public static Point operator +(in Point p, in Vector v)
    {
        return new Point(p.X + v.X, p.Y + v.Y, p.Z + v.Z);
    }

    /// <summary>
    /// Translates a point by subtracting a vector, returning the resulting point (p - v).
    /// </summary>
    /// <param name="p">The original point.</param>
    /// <param name="v">The vector to subtract.</param>
    /// <returns>
    /// A new <see cref="Point"/> resulting from translating the input point by the opposite of the vector.
    /// </returns>
    public static Point operator -(in Point p, in Vector v)
    {
        return new Point(p.X - v.X, p.Y - v.Y, p.Z - v.Z);
    }

    //this operation shouldn't be allowed
    /*public static Point operator +(in Vector a, in Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }*/

    /// <summary>
    /// Returns a new point, inverting all the coordinates of the specified point.
    /// </summary>
    /// <returns>
    /// A <see cref="Point"/> located at (-X, -Y, -Z).
    /// </returns>
    public static Point operator -(Point p)
    {
        return new Point(-p.X, -p.Y, -p.Z);
    }

    /// <summary>
    /// Subtracts two points and returns the displacement vector from <paramref name="b"/> to <paramref name="a"/>.
    /// </summary>
    /// <param name="a">The end point of the returned vector.</param>
    /// <param name="b">The origin of the returned vector.</param>
    /// <returns>A vector that points from <paramref name="b"/> to <paramref name="a"/>.</returns>
    public static Vector operator -(in Point a, in Point b)
    {
        return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static Point operator *(Point p, float alpha)
    {
        return new Point(p.X * alpha, p.Y * alpha, p.Z * alpha);
    }

    public static Point operator *(float alpha, Point p)
    {
        return new Point(p.X * alpha, p.Y * alpha, p.Z * alpha);
    }

    /// <summary>
    /// Converts this point into a vector originating from the origin.
    /// </summary>
    /// <returns>
    /// A <see cref="Vector"/> with the same components as this point.
    /// </returns>
    public Vector ToVector()
    {
        return new Vector(X, Y, Z);
    }
}