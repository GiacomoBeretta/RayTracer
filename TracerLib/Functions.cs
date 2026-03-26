namespace TracerLib;
public static class Functions
{
    public static bool AreClose(float a, float b, float epsilon = 1e-5f)
    {
        return Math.Abs(a - b) < epsilon;
    }
}