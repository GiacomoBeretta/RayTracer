// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// A class that represents a Normalized 3D Vector
/// </summary>
public struct Normal
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

    /// <summary>
    /// Returns a string representation of the vector in the format:
    /// Normal(x={X}, y={Y}, z={Z}).
    /// </summary>
    public override string ToString()
    {
        return $"Normal(x={X}, y={Y}, z={Z})";
    }

    public void Print()
    {
        Console.WriteLine(ToString());
    }

    /// <summary>
    /// Returns whether the two normals are approximately equal within a given tolerance.
    /// </summary>
    /// <param name="a">First normal vector.</param>
    /// <param name="b">Second normal vector.</param>
    /// <param name="epsilon">
    /// Tolerance threshold used for floating-point comparison of each component. Defaults to 1e-5.
    /// </param>
    /// <returns> True if the normals are equal within the given tolerance on all axes.</returns>
    public static bool _AreNormalsClose(Normal a, Normal b, float epsilon = 1e-5f)
    {
        return Functions.AreClose(a.X, b.X, epsilon)
               && Functions.AreClose(a.Y, b.Y, epsilon)
               && Functions.AreClose(a.Z, b.Z, epsilon);
    }

    /// <summary>
    /// Determines whether this vector is normalized, within a given numerical tolerance.
    /// </summary>
    /// <param name="epsilon">
    /// The tolerance used for floating-point comparison (default is 1e-5f).
    /// </param>
    /// <returns>
    /// True if the vector is approximately unit length; otherwise, false.
    /// </returns>
    public bool IsNormalized(float epsilon = 1e-5f)
    {
        return Functions.AreClose(1, SquaredNorm(), epsilon);
    }

    /// <summary>
    /// Validates that this vector is normalized within a given tolerance.
    /// </summary>
    /// <param name="epsilon">
    /// The numerical tolerance used to determine whether the vector is normalized (default is 1e-5f).
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the vector is not normalized within the specified tolerance.
    /// </exception>
    public void CheckNormalized(float epsilon = 1e-5f)
    {
        if (!IsNormalized(epsilon))
        {
            throw new ArgumentOutOfRangeException($"Normal is not normalized within epsilon={epsilon}");
        }
    }

    /// <summary>
    /// Negates the normal vector, reversing its direction.
    /// </summary>
    /// <returns>
    /// A new <see cref="Normal"/> pointing in the opposite direction.
    /// </returns>
    public static Normal operator -(Normal n)
    {
        return new Normal(-n.X, -n.Y, -n.Z);
    }

    /// <summary>
    /// Returns the scalar product between a <see cref="Normal"/> and a <see cref="Vector"/>.
    /// </summary>
    /// <param name="n"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static float operator *(Normal n, Vector v)
    {
        return n.X * v.X + n.Y * v.Y + n.Z * v.Z;
    }

    /// <summary>
    /// Returns the scalar product between a <see cref="Normal"/> and a <see cref="Vector"/>.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static float operator *(Vector v, Normal n)
    {
        return n * v;
    }

    /// <summary>
    /// Returns the cross product between a <see cref="Normal"/> and a <see cref="Vector"/>.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Vector CrossProduct(Vector v, Normal n)
    {
        return new Vector(v.Y * n.Z - v.Z * n.Y, v.Z * n.X - v.X * n.Z, v.X * n.Y - v.Y * n.X);
    }

    /// <summary>
    /// Returns the cross product between a <see cref="Normal"/> and a <see cref="Vector"/>.
    /// </summary>
    /// <param name="n"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector CrossProduct(Normal n, Vector v)
    {
        return new Vector(n.Y * v.Z - n.Z * v.Y, n.Z * v.X - n.X * v.Z, n.X * v.Y - n.Y * v.X);
    }

    /// <summary>
    /// Returns the cross product between 2 <see cref="Normal"/>.
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    public static Vector CrossProduct(Normal n1, Normal n2)
    {
        return new Vector(n1.Y * n2.Z - n1.Z * n2.Y, n1.Z * n2.X - n1.X * n2.Z, n1.X * n2.Y - n1.Y * n2.X);
    }

    /// <summary>
    /// Returns the squared norm of a <see cref="Normal"/> vector.
    /// </summary>
    /// <returns></returns>
    public float SquaredNorm()
    {
        return MathF.Pow(X, 2) + MathF.Pow(Y, 2) + MathF.Pow(Z, 2);
    }

    /// <summary>
    /// Returns the norm of a <see cref="Normal"/> vector.
    /// </summary>
    ///<remarks>
    /// If you want to compute the squared norm, use the SquaredNorm method, is more efficient.
    /// </remarks>
    /// <returns></returns>
    public float Norm()
    {
        return MathF.Sqrt(SquaredNorm());
    }

    /*/// <summary>
    /// Returns a normalized version of *this* <see cref="Normal"/> (that should already be normalized).
    /// </summary>
    /// <returns>A new normalized <see cref="Normal"/>. The original vector is not modified.</returns>
    public Normal Normalize()
    {
        float norm = Norm();
        return new Normal(X * 1 /norm, Y * 1 / norm, Z * 1 / norm);
    }*/

    /// <summary>
    /// Returns a <see cref="Vector"/> from this normal without modifying its values.
    /// </summary>
    /// <returns>
    /// A new <see cref="Vector"/> instance with identical X, Y, and Z components.
    /// </returns>
    public Vector ToVector()
    {
        return new Vector(X, Y, Z);
    }
}