using System.Diagnostics;
using System.Globalization;
using TracerLib;
using Xunit.Abstractions;

namespace TracerTests;

public class RayTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public RayTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TestAt()
    {
        Point origin = new Point(0f, 1f, 2f);
        Vector dir = new Vector(3f, 4f, 5f);
        float t = 10f;
        Ray ray = new Ray(origin, dir);

        Assert.True(Point._ArePointsClose(new Point(30f, 41f, 52f), ray.At(t)));
    }

    [Fact]
    public void TestTransform()
    {
        Ray ray = new Ray(new Point(1.0f, 2.0f, 3.0f), new Vector(6.0f, 5.0f, 4.0f));
        Transformation translation = new Transformation(new Vector(10.0f, 11.0f, 12.0f));
        Transformation rotation = new Transformation(Axis.X, MathF.PI / 2);
        Ray transformed = (translation * rotation) * ray;

        Assert.True(Point._ArePointsClose(new Point(11.0f, 8.0f, 14.0f), transformed.Origin));
        Assert.True(Vector._AreVectorsClose(new Vector(6.0f, -4.0f, 5.0f), transformed.Dir));
    }
}