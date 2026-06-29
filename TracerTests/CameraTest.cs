using TracerLib;
using Xunit.Abstractions;

namespace TracerTests;

public class CameraTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CameraTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void FireRayOrthogonalTestDimensions1_1()
    {
        OrthogonalCamera cam = new OrthogonalCamera(new Transformation());

        Ray ray1 = cam.FireRay(0.0f, 0.0f); // top-left corner
        Ray ray2 = cam.FireRay(1.0f, 0.0f); // top-right corner
        Ray ray3 = cam.FireRay(0.0f, 1.0f); // bottom-left corner
        Ray ray4 = cam.FireRay(1.0f, 1.0f); // bottom-right corner
        Ray ray5 = cam.FireRay(0.5f, 0.5f); // center

        //Verify that the rays are parallel to the ray pointing towards the x-axis.
        Vector xVector = new Vector(1, 0, 0);
        Assert.True(Vector._AreVectorsClose(xVector, ray1.Dir));
        Assert.True(Vector._AreVectorsClose(xVector, ray2.Dir));
        Assert.True(Vector._AreVectorsClose(xVector, ray3.Dir));
        Assert.True(Vector._AreVectorsClose(xVector, ray4.Dir));
        Assert.True(Vector._AreVectorsClose(xVector, ray5.Dir));

        //Verify that the rays are parallel between each other by vanishing the cross product
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray2.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray3.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray4.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray5.Dir).SquaredNorm()));

        //Verify that the rays pass through the corners and in the center of the image plane [-0.5,0.5]×[-0.5,0.5]
        Assert.True(Point._ArePointsClose(new Point(0.0f, 0.5f, 0.5f), ray1.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -0.5f, 0.5f), ray2.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, 0.5f, -0.5f), ray3.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -0.5f, -0.5f), ray4.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, 0, 0), ray5.At(1.0f)));
    }

    [Fact]
    public void FireRayOrthogonalTestDimensions5_3()
    {
        OrthogonalCamera cam = new OrthogonalCamera(new Transformation(), 5.0f, 3.0f);

        Ray ray1 = cam.FireRay(0.0f, 0.0f); // top-left corner
        Ray ray2 = cam.FireRay(1.0f, 0.0f); // top-right corner
        Ray ray3 = cam.FireRay(0.0f, 1.0f); // bottom-left corner
        Ray ray4 = cam.FireRay(1.0f, 1.0f); // bottom-right corner
        Ray ray5 = cam.FireRay(0.5f, 0.5f); // center

        //Verify that the rays are parallel to the ray pointing towards the x-axis.
        Vector xVector = new Vector(1, 0, 0);
        Assert.True(Vector._AreVectorsClose(xVector, ray1.Dir));
        Assert.True(Vector._AreVectorsClose(xVector, ray2.Dir));
        Assert.True(Vector._AreVectorsClose(xVector, ray3.Dir));
        Assert.True(Vector._AreVectorsClose(xVector, ray4.Dir));
        Assert.True(Vector._AreVectorsClose(xVector, ray5.Dir));

        //Verify that the rays are parallel between each other by vanishing the cross product
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray2.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray3.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray4.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray5.Dir).SquaredNorm()));

        //Verify that the rays pass through the corners and in the center of the image plane [-2.5,2.5]×[-1.5,1.5]
        Assert.True(Point._ArePointsClose(new Point(0.0f, 2.5f, 1.5f), ray1.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -2.5f, 1.5f), ray2.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, 2.5f, -1.5f), ray3.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -2.5f, -1.5f), ray4.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, 0, 0), ray5.At(1.0f)));
    }

    [Fact]
    public void FireRayOrthogonalTestDimensions1_1_Transformed()
    {
        Vector vTranslation = new Vector(-3, 0, 0);

        Transformation rotationx = new Transformation(Axis.X, MathF.PI / 2);
        Transformation translation = new Transformation(vTranslation);
        Transformation rotationy = new Transformation(Axis.Y, MathF.PI / 2);
        Transformation transformation1 = rotationx * rotationy * translation * rotationx;

        OrthogonalCamera cam1 = new OrthogonalCamera(transformation1);

        Ray ray1 = cam1.FireRay(0.0f, 0.0f); // top-left corner
        Ray ray2 = cam1.FireRay(1.0f, 0.0f); // top-right corner
        Ray ray3 = cam1.FireRay(0.0f, 1.0f); // bottom-left corner
        Ray ray4 = cam1.FireRay(1.0f, 1.0f); // bottom-right corner
        Ray ray5 = cam1.FireRay(0.5f, 0.5f); // center

        // the transformation1 is equivalent to transformation2
        rotationx = new Transformation(Axis.X, MathF.PI);
        Transformation rotationz = new Transformation(Axis.Z, MathF.PI / 2);
        Transformation transformation2 = rotationz * translation * rotationx;
        
        OrthogonalCamera cam2 = new OrthogonalCamera(transformation2);
        Ray ray6 = cam2.FireRay(0.0f, 0.0f);
        Ray ray7 = cam2.FireRay(1.0f, 0.0f);
        Ray ray8 = cam2.FireRay(0.0f, 1.0f);
        Ray ray9 = cam2.FireRay(1.0f, 1.0f);
        Ray ray10 = cam2.FireRay(0.5f, 0.5f);

        // Verify that trnasformation1 is equivalent to transformation2
        Assert.True(Ray._AreRaysClose(ray1, ray6));
        Assert.True(Ray._AreRaysClose(ray2, ray7));
        Assert.True(Ray._AreRaysClose(ray3, ray8));
        Assert.True(Ray._AreRaysClose(ray4, ray9));
        Assert.True(Ray._AreRaysClose(ray5, ray10));

        // Verify that the rays are parallel to the ray pointing towards the y-axis.
        Vector xVector = new Vector(0, 1, 0);
        Assert.True(Vector._AreVectorsClose(xVector, ray1.Dir));
        Assert.True(Vector._AreVectorsClose(xVector, ray2.Dir));
        Assert.True(Vector._AreVectorsClose(xVector, ray3.Dir));
        Assert.True(Vector._AreVectorsClose(xVector, ray4.Dir));
        Assert.True(Vector._AreVectorsClose(xVector, ray5.Dir));

        // Verify that the rays are parallel between each other by vanishing the cross product
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray2.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray3.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray4.Dir).SquaredNorm()));
        Assert.True(Functions.AreClose(0.0f, Vector.CrossProduct(ray1.Dir, ray5.Dir).SquaredNorm()));

        // Verify that the rays pass through the corners and in the center of the transformed image plane
        Assert.True(Point._ArePointsClose(new Point(0.5f, -3, -0.5f), ray1.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(-0.5f, -3, -0.5f), ray2.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.5f, -3, 0.5f), ray3.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(-0.5f, -3, 0.5f), ray4.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -3, 0), ray5.At(1.0f)));
    }

    [Fact]
    public void FireRayPerspectiveTestDimensions1_1()
    {
        PerspectiveCamera cam = new PerspectiveCamera(new Transformation());

        Ray ray1 = cam.FireRay(0.0f, 0.0f); // top-left corner
        Ray ray2 = cam.FireRay(1.0f, 0.0f); // top-right corner
        Ray ray3 = cam.FireRay(0.0f, 1.0f); // bottom-left corner
        Ray ray4 = cam.FireRay(1.0f, 1.0f); // bottom-right corner
        Ray ray5 = cam.FireRay(0.5f, 0.5f); // center

        // Verify that the rays originate in (-1,0,0)
        Point origin = new Point(-1, 0, 0);
        Assert.True(Point._ArePointsClose(origin, ray1.Origin));
        Assert.True(Point._ArePointsClose(origin, ray2.Origin));
        Assert.True(Point._ArePointsClose(origin, ray3.Origin));
        Assert.True(Point._ArePointsClose(origin, ray4.Origin));
        Assert.True(Point._ArePointsClose(origin, ray5.Origin));

        // Verify that the rays pass through the corners and in the center of the image plane [-0.5,0.5]×[-0.5,0.5]
        Assert.True(Point._ArePointsClose(new Point(0.0f, 0.5f, 0.5f), ray1.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -0.5f, 0.5f), ray2.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, 0.5f, -0.5f), ray3.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -0.5f, -0.5f), ray4.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, 0, 0), ray5.At(1.0f)));
    }

    [Fact]
    public void FireRayPerspectiveTestDimensions5_3_2()
    {
        PerspectiveCamera cam = new PerspectiveCamera(new Transformation(), 5.0f, 3.0f, 2.0f);

        Ray ray1 = cam.FireRay(0.0f, 0.0f); // top-left corner
        Ray ray2 = cam.FireRay(1.0f, 0.0f); // top-right corner
        Ray ray3 = cam.FireRay(0.0f, 1.0f); // bottom-left corner
        Ray ray4 = cam.FireRay(1.0f, 1.0f); // bottom-right corner
        Ray ray5 = cam.FireRay(0.5f, 0.5f); // center

        // Verify that the rays originate in (-2,0,0)
        Point origin = new Point(-2, 0, 0);
        Assert.True(Point._ArePointsClose(origin, ray1.Origin));
        Assert.True(Point._ArePointsClose(origin, ray2.Origin));
        Assert.True(Point._ArePointsClose(origin, ray3.Origin));
        Assert.True(Point._ArePointsClose(origin, ray4.Origin));
        Assert.True(Point._ArePointsClose(origin, ray5.Origin));

        // Verify that the rays pass through the corners and in the center of the image plane [-2.5,2.5]×[-1.5,1.5]
        Assert.True(Point._ArePointsClose(new Point(0.0f, 2.5f, 1.5f), ray1.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -2.5f, 1.5f), ray2.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, 2.5f, -1.5f), ray3.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -2.5f, -1.5f), ray4.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, 0, 0), ray5.At(1.0f)));
    }

    [Fact]
    public void FireRayPerspectiveTestDimensions1_1_Transformed()
    {
        Vector vTranslation = new Vector(-3, 0, 0);

        Transformation rotationx = new Transformation(Axis.X, MathF.PI / 2);
        Transformation translation = new Transformation(vTranslation);
        Transformation rotationy = new Transformation(Axis.Y, MathF.PI / 2);
        Transformation transformation1 = rotationx * rotationy * translation * rotationx;

        PerspectiveCamera cam1 = new PerspectiveCamera(transformation1, 1, 1, 5);

        Ray ray1 = cam1.FireRay(0.0f, 0.0f); // top-left corner
        Ray ray2 = cam1.FireRay(1.0f, 0.0f); // top-right corner
        Ray ray3 = cam1.FireRay(0.0f, 1.0f); // bottom-left corner
        Ray ray4 = cam1.FireRay(1.0f, 1.0f); // bottom-right corner
        Ray ray5 = cam1.FireRay(0.5f, 0.5f); // center

        // the transformation1 is equivalent to transformation2
        rotationx = new Transformation(Axis.X, MathF.PI);
        Transformation rotationz = new Transformation(Axis.Z, MathF.PI / 2);
        Transformation transformation2 = rotationz * translation * rotationx;
        
        PerspectiveCamera cam2 = new PerspectiveCamera(transformation2, 1, 1, 5);
        Ray ray6 = cam2.FireRay(0.0f, 0.0f);
        Ray ray7 = cam2.FireRay(1.0f, 0.0f);
        Ray ray8 = cam2.FireRay(0.0f, 1.0f);
        Ray ray9 = cam2.FireRay(1.0f, 1.0f);
        Ray ray10 = cam2.FireRay(0.5f, 0.5f);

        // Verify that trnasformation1 is equivalent to transformation2
        Assert.True(Ray._AreRaysClose(ray1, ray6));
        Assert.True(Ray._AreRaysClose(ray2, ray7));
        Assert.True(Ray._AreRaysClose(ray3, ray8));
        Assert.True(Ray._AreRaysClose(ray4, ray9));
        Assert.True(Ray._AreRaysClose(ray5, ray10));

        // Verify that the rays originate in (0,-8,0)
        Point origin = new Point(0, -8, 0);
        Assert.True(Point._ArePointsClose(origin, ray1.Origin));
        Assert.True(Point._ArePointsClose(origin, ray2.Origin));
        Assert.True(Point._ArePointsClose(origin, ray3.Origin));
        Assert.True(Point._ArePointsClose(origin, ray4.Origin));
        Assert.True(Point._ArePointsClose(origin, ray5.Origin));

        // Verify that the rays pass through the corners and in the center of the transformed image plane
        Assert.True(Point._ArePointsClose(new Point(0.5f, -3, -0.5f), ray1.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(-0.5f, -3, -0.5f), ray2.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.5f, -3, 0.5f), ray3.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(-0.5f, -3, 0.5f), ray4.At(1.0f)));
        Assert.True(Point._ArePointsClose(new Point(0.0f, -3, 0), ray5.At(1.0f)));
    }
}