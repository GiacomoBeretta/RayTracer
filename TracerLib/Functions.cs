// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

public static class Functions
{
    public static bool AreClose(float a, float b, float epsilon = 1e-5f)
    {
        return MathF.Abs(a - b) < epsilon;
    }
    
    public static bool AreArrayClose(float[] a, float[] b, float epsilon = 1e-5f)
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
}