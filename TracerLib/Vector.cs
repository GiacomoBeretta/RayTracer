// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// Represents a vector in 3D space.
/// </summary>
public struct Vector
{
    public float X { get; private set; }

    public float Y { get; private set; }

    public float Z { get; private set; }

    /// <summary>
    /// Initializes a zero vector.
    /// </summary>
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

    /// <summary>
    /// Determines whether two vectors are approximately equal within a given tolerance.
    /// </summary>
    /// <param name="v1">The first vector to compare.</param>
    /// <param name="v2">The second vector to compare.</param>
    /// <param name="epsilon">The tolerance used when comparing each coefficient of the vectors.</param>
    /// <returns>
    /// True if the absolute difference between corresponding coefficients of the two vectors
    /// is less than or equal to the specified tolerance; otherwise, false.
    /// </returns>
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

    /// <summary>
    /// Returns the scalar product between the two vectors.
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static float operator *(Vector v1, Vector v2)
    {
        return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
    }

    public static Normal CrossProduct(Vector v1, Vector v2)
    {
        return new Normal(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
    }

    /// <summary>
    /// Returns X<sup>2</sup> + Y<sup>2</sup> + Z<sup>2</sup>.
    /// </summary>
    /// <returns></returns>
    public float SquaredNorm()
    {
        return X * X + Y * Y + Z * Z;
    }

    /// <summary>
    /// Returns sqrt(X<sup>2</sup> + Y<sup>2</sup> + Z<sup>2</sup>).
    /// </summary>
    /// <remarks>
    /// If you want to compute the squared norm, use the SquaredNorm method, is more efficient.
    /// </remarks> 
    /// <returns></returns>
    public float Norm()
    {
        return MathF.Sqrt(X * X + Y * Y + Z * Z);
    }

    /// <summary>
    /// Normalizes the vector, dividing it by its norm.
    /// </summary>
    public void Normalize()
    {
        float norm = Norm();
        X = X / norm;
        Y = Y / norm;
        Z = Z / norm;
    }

    /// <summary>
    /// Returns a <see cref="Normal"/>, i.e. a normalized vector with same direction
    /// </summary>
    /// <returns></returns>
    public Normal ToNormal()
    {
        return new Normal(X / Norm(), Y / Norm(), Z / Norm());
    }
}