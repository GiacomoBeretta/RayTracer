// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

public class Normal
{
    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    public Normal(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static bool _AreCloseNormal(Normal a, Normal b)
    {
        return Functions.AreClose(a.X, b.X) && Functions.AreClose(a.Y, b.Y) && Functions.AreClose(a.Z, b.Z);
    }

    public Normal _NormalNegation()
    {
        return new Normal(-this.X, -this.Y, -this.Z);
    }

    public static Normal operator *(Normal n, float a)
    {
        return new Normal(a * n.X, a * n.Y, a * n.Z);
    }
    
    public static Normal operator *(float a, Normal n)
    {
        return n * a;
    }

    public static float operator *(Normal n, Vector v)
    {
        return n.X * v.X + n.Y * v.Y + n.Z * v.Z;
    }
    
    public static float operator *(Vector v, Normal n)
    {
        return n * v;
    }

    public static Vector CrossProduct(Vector v, Normal n)
    {
        return new Vector(v.Y*n.Z - v.Z*n.Y, v.Z*n.X - v.X*n.Z, v.X*n.Y - v.Y*n.X);
    }

    public static Vector CrossProduct(Normal n, Vector v)
    {
        return new Vector(n.Y*v.Z - n.Z*v.Y, n.Z*v.X - n.X*v.Z, n.X*v.Y - n.Y*v.X);
    }

    public static Vector CrossProduct(Normal n, Normal v)
    {
        return new Vector(n.Y*v.Z - n.Z*v.Y, n.Z*v.X - n.X*v.Z, n.X*v.Y - n.Y*v.X);
    }

    public float SquaredNorm()
    {
        return MathF.Pow(this.X, 2) + MathF.Pow(this.Y, 2) + MathF.Pow(this.Z, 2);
    }

    public float Norm()
    {
        return MathF.Sqrt(this.SquaredNorm());
    }

    public Normal Normalize()
    {
        return new Normal(this.X, this.Y, this.Z) * (1 / this.Norm());
    }
    
    
}