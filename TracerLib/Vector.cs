// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

//Forse è meglio fare dei template per le funzioni, così che vector e point ecc. abbiano già i metodi implementati?

public struct Vector
{
    public float X { get; private set; }

    public float Y { get; private set; }

    public float Z { get; private set; }

    public Vector()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }

    public Vector(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    public override string ToString()
    {
        return $"Vector(x={X}, y={Y}, z={Z})";
    }

    public void Print()
    {
        Console.WriteLine(ToString());
    }

    public static bool _AreVectorsClose(Vector v1, Vector v2, float epsilon = 1e-5f)
    {
        return Functions.AreClose(v1.X, v2.X, epsilon)
               && Functions.AreClose(v1.Y, v2.Y, epsilon)
               && Functions.AreClose(v1.Z, v2.Z, epsilon);
    }

    public static Vector operator +(Vector v1, Vector v2)
    {
        return new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    }

    public static Vector operator -(Vector v1)
    {
        return new Vector(-v1.X, -v1.Y, -v1.Z);
    }

    public static Vector operator -(Vector v1, Vector v2)
    {
        return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    }

    public static Vector operator *(Vector v1, float a)
    {
        return new Vector(v1.X * a, v1.Y * a, v1.Z * a);
    }

    //è meglio usare v1*a o scrivere di nuovo v1.X*a, v1.Y*a, v1.Z*a?
    public static Vector operator *(float a, Vector v1)
    {
        return new Vector(v1.X * a, v1.Y * a, v1.Z * a);
    }

    public static float operator *(Vector v1, Vector v2)
    {
        return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
    }
    
    public static Normal CrossProduct(Vector v1, Vector v2)
    {
        return new Normal(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
    }

    public float SquaredNorm()
    {
        return X * X + Y * Y + Z * Z;
    }

    /// <summary>
    /// If you want to compute the squared norm, use the SquaredNorm method, is more efficient.
    /// </summary>
    /// <returns></returns>
    public float Norm()
    {
        return MathF.Sqrt(X * X + Y * Y + Z * Z);
    }

    public void Normalize()
    {
        float norm = Norm();
        X = X / norm;
        Y = Y / norm;
        Z = Z / norm;
    }
    
    /// <summary>
    /// Returns a <c>Normal</c>, i.e. a normalized vector with same direction
    /// </summary>
    /// <returns></returns>
    public Normal ToNormal()
    {
        return new Normal(X/this.Norm(), Y/this.Norm(), Z/this.Norm());    
    }
}