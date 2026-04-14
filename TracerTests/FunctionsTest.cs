using TracerLib;

//namespace TracerTests;

public class FunctionsTest
{
    [Fact]
    public void TestAreClose()
    {
        float a = 4.687908f;
        float b = 4.687901f;
        float c = 4.68791f;
        Assert.True(Functions.AreClose(a, b));
        Assert.True(Functions.AreClose(a, c));

        float d = 4.687918f;
        Assert.False(Functions.AreClose(a, d));

        float e = 4.6876f;
        Assert.False(Functions.AreClose(a, e, 1e-4f));
        float f = 4.68799f;
        Assert.True(Functions.AreClose(a, f, 1e-4f));
    }

    [Fact]
    public void TestAreArrayClose()
    {
        float[] a = [2, 5.352f, 933, -39.6f];
        float[] b = [2, 5.355f, 933, -39.69f];

        Assert.False(Functions.AreArrayClose(a, b));
        Assert.True(Functions.AreArrayClose(a, b, 1e-1f));
    }

    [Fact]
    public void TestMatrix4X4Product()
    {
        float[] m1 =
        [
            2, 3, 1, 0.5f,
            3, -3, 0.25f, 1,
            1, 4, 7, 3,
            21, 6, 3, 2
        ];
        float[] m2 =
        [
            2, 1, 0, 0,
            5, 2.5f, 1, 4,
            0.5f, 1, 3, 6,
            0, 1, 6, 2
        ];

        float[] product =
        [
            19.5f, 11, 9, 19,
            -8.875f, -3.25f, 3.75f, -8.5f,
            25.5f, 21, 43, 64,
            73.5f, 41, 27, 46
        ];

        Assert.Equal(product, Functions.Matrix4X4Product(m1, m2));
    }
}