
using System.Runtime.InteropServices.JavaScript;
using TracerLib;
namespace TracerTests;


public class ShapeTest
{
    [Fact]
    public void TestCreateONB()
    {
        PCG pcg = new PCG();
        for (int i = 0; i < 100; i++)
        {
            float phi = pcg.RandomFloat() * 2 * MathF.PI;
            float theta = MathF.Acos(pcg.RandomFloat());
            
            float x = MathF.Sin(theta) * MathF.Cos(phi);
            float y = MathF.Sin(theta) * MathF.Sin(phi);
            float z = MathF.Cos(theta);
            Normal normal = new Normal(x,y,z);

            Vector e1, e2, e3;
            Shape.CreateONB(normal, out e1, out e2, out e3);
            
            Assert.Equal(normal.X, e3.X);
            Assert.Equal(normal.Y, e3.Y);
            Assert.Equal(normal.Z, e3.Z);
            
            Assert.True(Functions.AreClose(0, e1*e2));
            Assert.True(Functions.AreClose(0, e2*e3));
            Assert.True(Functions.AreClose(0, e3*e1));
            
            Assert.True(Functions.AreClose(1, e1.SquaredNorm()));
            Assert.True(Functions.AreClose(1, e2.SquaredNorm()));
            Assert.True(Functions.AreClose(1, e3.SquaredNorm()));
            
            Assert.True(Normal._AreNormalsClose(e3.ToNormal(), Vector.CrossProduct(e1, e2)));
        }
    }
}

public class SphereTest
{
    [Fact]
    public void TestSphereConstructor()
    {
        Sphere s = new Sphere();
        Assert.True(Transformation.AreTransformationsClose(new Transformation(), s.Transform));
    }

    [Fact]
    public void TestSphereConstructorWithTransformation()
    {
        Transformation t = new Transformation(2, 7, 0.5f);
        Sphere s = new Sphere(t);
        Assert.Equal(t, s.Transform);
    }

    [Fact]
    public void TestIsCloseTo_Sphere()
    {
        Sphere sphere1 = new Sphere(new Transformation(1, 2, 3.000016f));
        Sphere sphere2 = new Sphere(new Transformation(1, 2, 3.00001f));

        Assert.True(sphere1._IsCloseTo(sphere2));
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
        Assert.Equal(new Normal(0, 0, 1), hit?.SurfaceNormal);
        Assert.Equal(new Vector2D(0, 0), hit?.SurfacePoint);
        Assert.Equal(ray, hit?.IncomingRay);
        Assert.Equal(1, hit?.T);

        ray = new Ray(new Point(3, 0, 0), new Vector(-1, 0, 0));
        hit = sphere.RayIntersection(ray);
        Assert.Equal(new Point(1, 0, 0), hit?.WorldPoint);
        Assert.Equal(new Normal(1, 0, 0), hit?.SurfaceNormal);
        Assert.Equal(new Vector2D(0, 0.5f), hit?.SurfacePoint);
        Assert.Equal(ray, hit?.IncomingRay);
        Assert.Equal(2, hit?.T);

        ray = new Ray(new Point(0, 0, 0), new Vector(1, 0, 0));
        hit = sphere.RayIntersection(ray);
        Assert.Equal(new Point(1, 0, 0), hit?.WorldPoint);
        Assert.Equal(new Normal(-1, 0, 0), hit?.SurfaceNormal);
        Assert.Equal(new Vector2D(0, 0.5f), hit?.SurfacePoint);
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

        Ray ray2 = new Ray(new Point(13, 0, 0), new Vector(-1, 0, 0));
        hit = sphere.RayIntersection(ray2);
        Assert.Equal(new Point(11, 0, 0), hit?.WorldPoint);
        Assert.Equal(new Normal(1, 0, 0), hit?.SurfaceNormal);
        Assert.Equal(new Vector2D(0, 0.5f), hit?.SurfacePoint);
        Assert.Equal(ray2, hit?.IncomingRay);
        Assert.Equal(2, hit?.T);

        Ray ray3 = new Ray(new Point(0, 0, 2), new Vector(0, 0, -1));
        hit = sphere.RayIntersection(ray3);
        Assert.Null(hit);
        
        Ray ray4 = new Ray(new Point(-10, 0, 0), new Vector(0, 0, -1));
        hit = sphere.RayIntersection(ray4);
        Assert.Null(hit);
    }
}

public class PlaneTest
{
    [Fact]
    public void TestPlaneConstructor()
    {
        var plane = new Plane();
        Assert.True(Transformation.AreTransformationsClose(new Transformation(), plane.Transform));
    }

    [Fact]
    public void TestPlaneConstructorWithTransformation()
    {
        var transformation = new Transformation('x', MathF.PI / 2);
        var plane = new Plane(transformation);

        Assert.True(Transformation.AreTransformationsClose(transformation, plane.Transform));
    }

    [Fact]
    public void TestPlaneNormal()
    {
        var plane = new Plane();

        var d1 = new Vector(6, 0, -4);
        var d2 = new Vector(73, 5, 48);

        Assert.True(Normal._AreNormalsClose(new Normal(0, 0, 1), plane._PlaneNormal(d1)));
        Assert.True(Normal._AreNormalsClose(new Normal(0, 0, -1), plane._PlaneNormal(d2)));
    }

    [Fact]
    public void TestPlanePointToUV()
    {
        var plane = new Plane();

        var p1 = new Point(0, 0, 1.0f);
        Assert.True(Functions.AreClose(0, plane._PlanePointToUV(p1).U));
        Assert.True(Functions.AreClose(0, plane._PlanePointToUV(p1).V));

        var p2 = new Point(2.3f, 5.7f, 0);
        Assert.True(Functions.AreClose(0.3f, plane._PlanePointToUV(p2).U));
        Assert.True(Functions.AreClose(0.7f, plane._PlanePointToUV(p2).V));
    }
}