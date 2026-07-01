using TracerLib;

namespace TracerTests;

public class Vector2DTest
{
    [Fact]
    public void TestConstructor()
    {
        Vector2D v1 = new Vector2D(9.33409f, 5.67803f);
        Assert.Equal(9.33409f, v1.U);
        Assert.Equal(5.67803f, v1.V);
    }

    [Fact]
    public void TestToString()
    {
        Vector2D v = new Vector2D(3, 5);
        Assert.Equal("Vector2D(u=3, v=5)", v.ToString());
    }

    [Fact]
    public void TestAreVectorsClose()
    {
        Vector2D v1 = new Vector2D(9.33409f, 5.67803f);
        Vector2D v2 = new Vector2D(9.334096f, 5.678034f);
        Assert.True(Vector2D._AreVectorsClose(v1, v2));

        Vector2D v3 = new Vector2D(9.33409f, 5.67805f);
        Assert.False(Vector2D._AreVectorsClose(v3, v2));
    }
}