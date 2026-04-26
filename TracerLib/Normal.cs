// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// The class Normal create a Normal 3D Vector
/// </summary>
public struct Normal
{
    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    // Constructor -start
    public Normal(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    // Constructor -end

    /// <summary>
    /// This function return a string showing the component of a <c>Normal</c> type variable 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"Normal(x={X}, y={Y}, z={Z})";
    }

    public void Print()
    {
        Console.WriteLine(ToString());
    }

    /// <summary>
    /// Closeness criterion between two <c>Normal</c> type variables 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool _AreCloseNormal(Normal a, Normal b, float epsilon = 1e-5f)
    {
        return Functions.AreClose(a.X, b.X, epsilon) && Functions.AreClose(a.Y, b.Y, epsilon) &&
               Functions.AreClose(a.Z, b.Z, epsilon);
    }

    /// <summary>
    /// Returns the negated <c>Normal</c> vector
    /// </summary>
    /// <returns></returns>
    public static Normal operator -(Normal n)
    {
        return new Normal(-n.X, -n.Y, -n.Z);
    }

    /// <summary>
    /// Returns the product per component between a <c>Normal</c> and a floating-point scalar
    /// </summary>
    /// <param name="n"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Normal operator *(Normal n, float a)
    {
        return new Normal(a * n.X, a * n.Y, a * n.Z);
    }

    /// <summary>
    /// Returns the product per component between a <c>Normal</c> and a floating-point scalar
    /// </summary>
    /// <param name="a"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Normal operator *(float a, Normal n)
    {
        return n * a;
    }

    /// <summary>
    /// Returns the scalar product between a <c>Normal</c> and a <c>Vector</c>
    /// </summary>
    /// <param name="n"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static float operator *(Normal n, Vector v)
    {
        return n.X * v.X + n.Y * v.Y + n.Z * v.Z;
    }

    /// <summary>
    /// Returns the scalar product between a <c>Normal</c> and a <c>Vector</c>
    /// </summary>
    /// <param name="v"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static float operator *(Vector v, Normal n)
    {
        return n * v;
    }

    /// <summary>
    /// Returns the cross product between a <c>Normal</c> and a <c>Vector</c>
    /// </summary>
    /// <param name="v"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Vector CrossProduct(Vector v, Normal n)
    {
        return new Vector(v.Y * n.Z - v.Z * n.Y, v.Z * n.X - v.X * n.Z, v.X * n.Y - v.Y * n.X);
    }

    /// <summary>
    /// Returns the cross product between a <c>Normal</c> and a <c>Vector</c>
    /// </summary>
    /// <param name="n"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector CrossProduct(Normal n, Vector v)
    {
        return new Vector(n.Y * v.Z - n.Z * v.Y, n.Z * v.X - n.X * v.Z, n.X * v.Y - n.Y * v.X);
    }

    /// <summary>
    /// Returns the cross product between 2 <c>Normal</c>
    /// </summary>
    /// <param name="n"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector CrossProduct(Normal n, Normal v)
    {
        return new Vector(n.Y * v.Z - n.Z * v.Y, n.Z * v.X - n.X * v.Z, n.X * v.Y - n.Y * v.X);
    }

    /// <summary>
    /// Returns the squared norm of a <c>Normal</c> variable
    /// </summary>
    /// <returns></returns>
    public float SquaredNorm()
    {
        return MathF.Pow(this.X, 2) + MathF.Pow(this.Y, 2) + MathF.Pow(this.Z, 2);
    }

    /// <summary>
    /// If you want to compute the squared norm, use the SquaredNorm method, is more efficient.
    /// </summary>
    /// <returns></returns>
    public float Norm()
    {
        return MathF.Sqrt(this.SquaredNorm());
    }

    /// <summary>
    /// Returns a Normalized Normal 
    /// </summary>
    /// <returns></returns>
    public Normal Normalize()
    {
        return new Normal(this.X, this.Y, this.Z) * (1 / this.Norm());
    }
}