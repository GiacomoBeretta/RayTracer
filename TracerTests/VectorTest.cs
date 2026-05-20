using TracerLib;

namespace TracerTests;

public class VectorTest
{
    [Fact]
    public void TestToString()
    {
        Vector v = new Vector(3, 5, 2);
        Assert.Equal("Vector(x=3, y=5, z=2)", v.ToString());
    }

    [Fact]
    public void TestAreVectorsClose()
    {
        Vector v1 = new Vector(9.33409f, 5.67803f, 8.34775f);
        Vector v2 = new Vector(9.334096f, 5.678034f, 8.347751f);
        //Console.WriteLine(Vector._AreVectorsClose(v1, v2));
        Assert.True(Vector._AreVectorsClose(v1, v2));

        Vector v3 = new Vector(9.33409f, 5.67803f, 8.34774f);
        Assert.False(Vector._AreVectorsClose(v3, v2));
    }

    [Fact]
    public void TestVectorSum()
    {
        Vector v1 = new Vector(2, 5, 10);
        Vector v2 = new Vector(6, 11, 201);
        Assert.Equal(new Vector(8, 16, 211), v1 + v2);
    }

    [Fact]
    public void TestMinusSign()
    {
        Vector v = new Vector(-2, -23, 380);
        Assert.Equal(new Vector(2, 23, -380), -v);
    }

    [Fact]
    public void TestSubtract()
    {
        Vector v1 = new Vector(27, 511, 5290);
        Vector v2 = new Vector(6, 12, 49);
        Assert.Equal(new Vector(21, 499, 5241), v1 - v2);
    }

    [Fact]
    public void TestFloatProduct()
    {
        Vector v1 = new Vector(3, 502, 21);
        float a = 2;
        Assert.Equal(new Vector(6, 1004, 42), v1 * a);
        Assert.Equal(a * v1, v1 * a);
        float b = 3.2f;
        Assert.True(Vector._AreVectorsClose(new Vector(9.6f, 1606.4f, 67.2f), v1 * b));
        Assert.Equal(b * v1, v1 * b);
    }

    [Fact]
    public void TestScalarProduct()
    {
        Vector v1 = new Vector(8, 5, 38);
        Vector v2 = new Vector(-1, 2, 10);
        Assert.Equal(382, v1 * v2);

        Vector v3 = new Vector(1.5f, -4.35f, 5.9f);
        Assert.True(Functions.AreClose(214.45f, v1 * v3));
    }

    [Fact]
    public void TestCrossProduct()
    {
        Vector v1 = new Vector(1, 2, 3);
        Vector v2 = new Vector(4, 6, 8);

        Assert.Equal(new Normal(-2, 4, -2), Vector.CrossProduct(v1, v2));
        Assert.Equal(new Normal(2, -4, 2), Vector.CrossProduct(v2, v1));
    }

    [Fact]
    public void TestSquaredNorm()
    {
        Vector v1 = new Vector(3, 5, 20);
        Assert.Equal(434, v1.SquaredNorm());
    }

    [Fact]
    public void TestNorm()
    {
        Vector v1 = new Vector(4.4f, 519.34f, 19.6f);
        Assert.True(Functions.AreClose(519.72834789f, v1.Norm(), 1e-3f));
    }

    [Fact]
    public void TestNormalize()
    {
        Vector v = new Vector(82.5f, 7.1f, 91.43f);
        v.Normalize();
        Assert.True(Vector._AreVectorsClose(new Vector(0.668809f, 0.05755812f, 0.741202f), v));
    }

    [Fact]
    public void TestToNormal()
    {
        Vector v = new Vector(3, 5, 20);
        float norm = v.Norm();
        Normal n = new Normal(3/norm, 5/norm, 20/norm);
        Assert.Equal(n, v.ToNormal());
    }
}