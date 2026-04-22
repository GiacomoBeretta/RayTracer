// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

public struct Point
{
    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    public Point(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public override string ToString()
    {
        var point = "Point(x=" + X + ", y=" + Y +", z=" + Z + ")";
        return point;
    }

    public void Print()
    {
        Console.WriteLine(ToString());
    }

    public static bool _AreClosePoint(Point a, Point b)
    {
        return Functions.AreClose(a.X, b.X) && Functions.AreClose(a.Y, b.Y) && Functions.AreClose(a.Z, b.Z);
    }

    public static Point operator +(in Point a, in Vector b)
    {
        return new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static Point operator +(in Vector a, in Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static Point operator -(Point p)
    {
        return new Point(-p.X, -p.Y, -p.Z);
    }

    public static Vector operator -(in Point a, in Point b)
    {
        return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static Point operator -(in Point a, in Vector b)
    {
        return new Point(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
    
    public static Point operator *(Point p, float alpha)
    {
        return new Point(p.X * alpha, p.Y * alpha, p.Z * alpha);
    }

    public static Point operator *(float alpha, Point p)
    {
        return new Point(p.X * alpha, p.Y * alpha, p.Z * alpha);
    }

    public Vector ToVector()
    {
        return new Vector(this.X, this.Y, this.Z);
    }
}