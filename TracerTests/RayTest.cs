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
        var origin = new Point(0f, 1f, 2f);
        var dir = new Vector(3f, 4f, 5f);
        var t = 10f;
        var ray = new Ray(origin, dir);
        
        Assert.True(Point._ArePointsClose(new Point(30f, 41f, 52f), ray.At(t)));
    }

    [Fact]
    public void TestTransform()
    {
        var ray = new Ray(new Point(1.0f, 2.0f, 3.0f), new Vector(6.0f, 5.0f, 4.0f));
        var translation = new Transformation(new Vector(10.0f, 11.0f, 12.0f));
        var rotation = new Transformation('x', MathF.PI/2);
        var transformed = ray * (translation * rotation);
        
        Assert.True(Point._ArePointsClose(new Point(11.0f, 8.0f, 14.0f), transformed.Origin));
        Assert.True(Vector._AreVectorsClose(new Vector(6.0f, -4.0f, 5.0f), transformed.Dir));
    }
}