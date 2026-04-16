using TracerLib;

namespace TracerTests;

public class RayTest
{
    [Fact]
    public void TestAt()
    {
        var origin = new Point(0f, 1f, 2f);
        var dir = new Vector(3f, 4f, 5f);
        var t = 10f;
        var ray = new Ray(origin, dir);
        
        Assert.True(Point._AreClosePoint(new Point(30f, 41f, 52f), ray.At(t)));
    }
}