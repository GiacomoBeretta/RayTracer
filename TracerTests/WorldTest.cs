#nullable disable

using TracerLib;

namespace TracerTests;

public class WorldTest
{
    [Fact]
    public void TestRayIntersection()
    {
        World world = new World();

        Sphere s1 = new Sphere(new Transformation(new Vector(2f, 0f, 0f)));
        Sphere s2 = new Sphere(new Transformation(new Vector(8f, 0f, 0f)));
        
        world.Add(s1);
        world.Add(s2);

        HitRecord? i1 = world.FindIntersection(new Ray(new Point(0f, 0f, 0f), new Vector(1f, 0f, 0f)));
        HitRecord? i2 = world.FindIntersection(new Ray(new Point(10f, 0f, 0f), new Vector(-1f, 0f, 0f)));
        
        Assert.True(Point._ArePointsClose(new Point(1f, 0f, 0f), i1.Value.WorldPoint));
        Assert.True(Point._ArePointsClose(new Point(9f, 0f, 0f), i2.Value.WorldPoint));
    }
}