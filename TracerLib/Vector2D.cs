// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// A 2D Vector to represent a point of a <c>Shape</c> surface
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
        return $"({U}, {V})";
    }

    public void Print()
    {
        Console.WriteLine(ToString());
    }

    public static bool _AreVectorsClose(Vector2D v1, Vector2D v2, float epsilon = 1e-5f)
    {
        return Functions.AreClose(v1.U, v2.U, epsilon)
               && Functions.AreClose(v1.V, v2.V, epsilon);
    }
}