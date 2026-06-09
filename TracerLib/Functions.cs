// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

public static class Functions
{
    /// <summary>
    /// Closeness criterion between two float scalars
    /// </summary>
    /// <param name="a">First scalar</param>
    /// <param name="b">Second scalar</param>
    /// <param name="epsilon">Closeness parameter</param>
    /// <returns></returns>
    public static bool AreClose(float a, float b, float epsilon = 1e-5f)
    {
        return MathF.Abs(a - b) < epsilon;
    }
    
    /// <summary>
    /// Closeness criterion between two float arrays
    /// </summary>
    /// <param name="a">First array</param>
    /// <param name="b">Second array</param>
    /// <param name="epsilon">Closeness parameter</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static bool AreArraysClose(float[] a, float[] b, float epsilon = 1e-5f)
    {
        int length = a.Length;
        if (length != b.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(b), b, "the "+ nameof(b)+ " array has not the same length as the "+nameof(a)+ " array");
        }
        bool areArrayClose = true;
        for (int i = 0; i < length; i++)
        {
            areArrayClose = areArrayClose
                             && Functions.AreClose(a[i], b[i], epsilon);
        }

        return areArrayClose;
    }

    /// <summary>
    /// Convert a floating point degree into radian
    /// </summary>
    /// <param name="deg">Degree value</param>
    /// <returns></returns>
    public static float DegToRad(float deg)
    {
        return deg * MathF.PI / 180f;
    }
}