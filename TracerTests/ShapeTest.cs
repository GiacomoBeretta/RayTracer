using TracerLib;

namespace TracerTests;

public class SphereTest
{
    [Fact]
    public void TestConstructor()
    {
        Sphere s = new Sphere();
        Assert.True(Transformation.AreTransformationsClose(new Transformation(), s.Transform));
    }

    [Fact]
    public void TestConstructorWithTransformation()
    {
        Transformation t = new Transformation(2, 7, 0.5f);
        Sphere s = new Sphere(t);
        Assert.Equal(t, s.Transform);
    }

    [Fact]
    public void TestSphereNormal()
    {
        Sphere sphere = new Sphere();

        Vector v = new Vector(1, 1, 1);
        Point p1 = new Point(-1, 0, 0);
        Assert.Equal(new Normal(-1, 0, 0), sphere._SphereNormal(p1, v));

        Point p2 = new Point(0, 1, 1);
        Assert.Equal(new Normal(0, -1, -1), sphere._SphereNormal(p2, v));
    }

    [Fact]
    public void TestSpherePointToUV()
    {
        Sphere sphere = new Sphere();

        Point p1 = new Point(0, 0, -1);
        Assert.Equal(1, sphere._SpherePointToUV(p1).V);

        Point p2 = new Point(0, 1, 0);
        Assert.Equal(0.25f, sphere._SpherePointToUV(p2).U);
    }

    [Fact]
    public void TestRayIntersectionWithUnitSphere()
    {
        Ray ray = new Ray(new Point(0, 0, 2), new Vector(0, 0, -1));
        Sphere sphere = new Sphere();
        HitRecord? hit = sphere.RayIntersection(ray);
        Assert.Equal(new Point(0, 0, 1), hit?.WorldPoint);
        Assert.Equal(new Normal(0,0,1), hit?.SurfaceNormal);
        Assert.Equal(new Vector2D(0,0), hit?.SurfacePoint);
        Assert.Equal(ray, hit?.IncomingRay);
        Assert.Equal(1, hit?.T);
        
        ray = new Ray(new Point(3, 0, 0), new Vector(-1, 0, 0));
        hit = sphere.RayIntersection(ray);
        Assert.Equal(new Point(1, 0, 0), hit?.WorldPoint);
        Assert.Equal(new Normal(1,0,0), hit?.SurfaceNormal);
        Assert.Equal(new Vector2D(0,0.5f), hit?.SurfacePoint);
        Assert.Equal(ray, hit?.IncomingRay);
        Assert.Equal(2, hit?.T);
        
        ray = new Ray(new Point(0, 0, 0), new Vector(1, 0, 0));
        hit = sphere.RayIntersection(ray);
        Assert.Equal(new Point(1, 0, 0), hit?.WorldPoint);
        Assert.Equal(new Normal(-1,0,0), hit?.SurfaceNormal);
        Assert.Equal(new Vector2D(0,0.5f), hit?.SurfacePoint);
        Assert.Equal(ray, hit?.IncomingRay);
        Assert.Equal(1, hit?.T);

        ray = new Ray(new Point(1, 1, 1), new Vector(1, 1, 1));
        hit = sphere.RayIntersection(ray);
        Assert.Null(hit);
    }

    [Fact]
    public void TestRayIntersectionWithTranslatedSphere()
    {
        Ray ray = new Ray(new Point(10, 0, 2), new Vector(0, 0, -1));

        Vector translateVector = new Vector(10, 0, 0);
        Transformation translate = new Transformation(translateVector);
        Sphere sphere = new Sphere(translate);

        HitRecord? hit = sphere.RayIntersection(ray);
        Assert.Equal(new Point(10, 0, 1), hit?.WorldPoint);
        Assert.Equal(new Normal(0, 0, 1), hit?.SurfaceNormal);
        Assert.Equal(new Vector2D(0, 0), hit?.SurfacePoint);
        Assert.Equal(ray, hit?.IncomingRay);
        Assert.Equal(1, hit?.T);
    }
}