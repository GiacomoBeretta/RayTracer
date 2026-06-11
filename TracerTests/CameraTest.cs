using TracerLib;

namespace TracerTests;

public class CameraTest
{
    [Fact]
    public void FireRayOrthogonalTest()
    {
        OrthogonalCamera cam = new OrthogonalCamera(new Transformation(), 2.0f); //Cercare metodo default

        Ray ray1 = cam.FireRay(0.0f, 0.0f);
        Ray ray2 = cam.FireRay(1.0f, 0.0f);
        Ray ray3 = cam.FireRay(0.0f, 1.0f);
        Ray ray4 = cam.FireRay(1.0f, 1.0f);
        
        //Verify that two rays are parallel by vanishing the cross product
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray2.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray3.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray4.Dir).SquaredNorm()));
        
        Assert.True(Point._ArePointsClose(ray1.At(1.0f), new Point(0.0f, 2.0f, -1.0f)));
        Assert.True(Point._ArePointsClose(ray2.At(1.0f), new Point(0.0f, -2.0f, -1.0f)));
        Assert.True(Point._ArePointsClose(ray3.At(1.0f), new Point(0.0f, 2.0f, 1.0f)));
        Assert.True(Point._ArePointsClose(ray4.At(1.0f), new Point(0.0f, -2.0f, 1.0f)));
    }

    [Fact]
    public void FireRayPerspectiveTest()
    {
        PerspectiveCamera cam = new PerspectiveCamera(new Transformation(), 1f, 2f);
        
        Ray ray1 = cam.FireRay(0.0f, 0.0f);
        Ray ray2 = cam.FireRay(1.0f, 0.0f);
        Ray ray3 = cam.FireRay(0.0f, 1.0f);
        Ray ray4 = cam.FireRay(1.0f, 1.0f);
        
        Assert.True(Point._ArePointsClose(ray1.Origin, ray2.Origin));
        Assert.True(Point._ArePointsClose(ray1.Origin, ray3.Origin));
        Assert.True(Point._ArePointsClose(ray1.Origin, ray4.Origin));
        
        ray1.At(1.0f).Print();
        
        Assert.True(Point._ArePointsClose(ray1.At(1.0f), new Point(0.0f, 2.0f, -1.0f)));
        Assert.True(Point._ArePointsClose(ray2.At(1.0f), new Point(0.0f, -2.0f, -1.0f)));
        Assert.True(Point._ArePointsClose(ray3.At(1.0f), new Point(0.0f, 2.0f, 1.0f)));
        Assert.True(Point._ArePointsClose(ray4.At(1.0f), new Point(0.0f, -2.0f, 1.0f)));
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