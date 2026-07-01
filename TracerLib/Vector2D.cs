// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// A 2D Vector to represent a point of a <see cref="Shape"/> surface.
/// </summary>
public struct Vector2D
{
    public float U { get; private set; }

    public float V { get; private set; }

    public Vector2D(float u, float v)
    {
        U = u;
        V = v;
    }

    public override string ToString()
    {
        return $"Vector2D(u={U}, v={V})";
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
    public static bool _AreVectorsClose(Vector2D v1, Vector2D v2, float epsilon = 1e-5f)
    {
        return Functions.AreClose(v1.U, v2.U, epsilon)
               && Functions.AreClose(v1.V, v2.V, epsilon);
    }
}