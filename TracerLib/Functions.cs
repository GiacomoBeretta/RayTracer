namespace TracerLib;

public static class Functions
{
    public static bool AreClose(float a, float b, float epsilon = 1e-5f)
    {
        return MathF.Abs(a - b) < epsilon;
    }

    public static bool AreArrayClose(float[] a, float[] b, float epsilon = 1e-5f)
    {
        bool areArrayClose = true;
        for (int i = 0; i < a.Length; i++)
        {
            areArrayClose = areArrayClose
                            && Functions.AreClose(a[i], b[i], epsilon);
        }

        return areArrayClose;
    }

    //aggiungere un check sulla dimensione di m1 e m2?
    public static float[] Matrix4X4Product(float[] m1, float[] m2)
    {
        float[] m3 = new float[16];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    m3[i * 4 + j] += m1[i * 4 + k] * m2[j + k * 4];
                }
            }
        }

        return m3;
    }

    /*
    public static float Matrix4X4Determinant(float[] m)
    {

    }*/
}