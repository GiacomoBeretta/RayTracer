using TracerLib;

namespace TracerTests;

public class CameraTest
{
    [Fact]
    public void FireRayOrthogonalTest()
    {
        OrthogonalCamera cam = new OrthogonalCamera(new Transformation(), 2.0f);

        Ray ray1 = cam.FireRay(0.0f, 0.0f);
        Ray ray2 = cam.FireRay(1.0f, 0.0f);
        Ray ray3 = cam.FireRay(0.0f, 1.0f);
        Ray ray4 = cam.FireRay(1.0f, 1.0f);
        Ray ray5 = cam.FireRay(0.5f, 0.5f);

        //Verify that the rays are parallel to the ray pointing towards the x-axis.
        Vector x_vector = new Vector(1, 0, 0);
        Assert.True(Vector._AreVectorsClose(x_vector, ray1.Dir));
        Assert.True(Vector._AreVectorsClose(x_vector, ray2.Dir));
        Assert.True(Vector._AreVectorsClose(x_vector, ray3.Dir));
        Assert.True(Vector._AreVectorsClose(x_vector, ray4.Dir));
        Assert.True(Vector._AreVectorsClose(x_vector, ray5.Dir));
        
        //Verify that the rays are parallel between each other by vanishing the cross product
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray2.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray3.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray4.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray5.Dir).SquaredNorm()));

        Assert.True(Point._ArePointsClose(new Point(0.0f, 2f, 1f), ray1.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -2f, 1f), ray2.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, 2f, -1f), ray3.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -2f, -1f), ray4.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, 0, 0), ray5.At(1.0f)));
    }

    [Fact]
    public void FireRayPerspectiveTest()
    {
        PerspectiveCamera cam = new PerspectiveCamera(new Transformation(), 1f, 2f);

        Ray ray1 = cam.FireRay(0.0f, 0.0f);
        Ray ray2 = cam.FireRay(1.0f, 0.0f);
        Ray ray3 = cam.FireRay(0.0f, 1.0f);
        Ray ray4 = cam.FireRay(1.0f, 1.0f);
        Ray ray5 = cam.FireRay(0.5f, 0.5f);

        Point origin = new Point(-1, 0, 0);
        Assert.True(Point._ArePointsClose(origin, ray1.Origin));
        Assert.True(Point._ArePointsClose(origin, ray2.Origin));
        Assert.True(Point._ArePointsClose(origin, ray3.Origin));
        Assert.True(Point._ArePointsClose(origin, ray4.Origin));
        Assert.True(Point._ArePointsClose(origin, ray5.Origin));

        Assert.True(Point._ArePointsClose(new Point(0.0f, 2f, 1f), ray1.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -2f, 1f), ray2.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, 2f, -1f), ray3.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -2f, -1f), ray4.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, 0, 0), ray5.At(1.0f)));
    }

    [Fact]
    public void OrthogonalTransformTest()
    {
        OrthogonalCamera cam = new OrthogonalCamera(transformation: new Transformation(new Vector(0.0f, -2.0f, 0.0f)) *
                                                                    new Transformation('z', MathF.PI / 2));
        Ray ray = cam.FireRay(0.5f, 0.5f);

        Assert.True(Point._ArePointsClose(ray.At(1.0f), new Point(0.0f, -2.0f, 0.0f)));
    }

    [Fact]
    public void PerspectiveTransformTest()
    {
        OrthogonalCamera cam = new OrthogonalCamera(transformation: new Transformation(new Vector(0.0f, -2.0f, 0.0f)) *
                                                                    new Transformation('z', MathF.PI / 2));
        Ray ray = cam.FireRay(0.5f, 0.5f);

        Assert.True(Point._ArePointsClose(ray.At(1.0f), new Point(0.0f, -2.0f, 0.0f)));
    }
}