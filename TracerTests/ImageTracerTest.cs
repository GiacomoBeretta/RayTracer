using TracerLib;
using Xunit.Abstractions;

namespace TracerTests;

public class ImageTracerTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private HDRImage image;
    private Camera camera;
    private ImageTracer tracer;
    
    [Fact]
    public void TestChecksInConstructor()
    {
        image = new HDRImage(4, 2);
        camera = new PerspectiveCamera(new Transformation(),2,1);
        tracer = new ImageTracer(image, camera);
        
        camera = new PerspectiveCamera(new Transformation(), 1, 0.5f, 2);
        tracer = new ImageTracer(image, camera);
        
        // assert that a negative pixel Side Subdivisions is not permitted
        Assert.Throws<ArgumentOutOfRangeException>(() => new ImageTracer(image, camera, null, 0));
        
        // assert that image plane and pixel grid must not have different aspect ratios.
        camera = new PerspectiveCamera(new Transformation(), 2, 2, 2);
        Assert.Throws<ArgumentOutOfRangeException>(() => new ImageTracer(image, camera));
    }
    
    [Fact]
    public void Test_uv_mapping()
    {
        image = new HDRImage(4, 2);
        camera = new PerspectiveCamera(new Transformation(), 8, 4, 2);
        tracer = new ImageTracer(image, camera);
        Ray ray = tracer.FireRayAtPixel(0, 0, 0, 0);
        Point expectedPoint = new Point(0, camera.Width/2, camera.Height/2); // top left of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));

        ray = tracer.FireRayAtPixel(3, 0, 1, 0);
        expectedPoint = new Point(0, -camera.Width/2, camera.Height/2); // top right of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));

        ray = tracer.FireRayAtPixel(0, 1, 0, 1);
        expectedPoint = new Point(0, camera.Width/2, -camera.Height/2); // bottom left of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));

        ray = tracer.FireRayAtPixel(3, 1, 1, 1);
        expectedPoint = new Point(0, -camera.Width/2, -camera.Height/2); // bottom right of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));
    }

    [Fact]
    public void TestFireRay_uv_PixelSubmapping()
    {
        image = new HDRImage(4, 2);
        camera = new PerspectiveCamera(new Transformation(), 8, 4, 2);
        // each pixel has dimension 2×2
        tracer = new ImageTracer(image, camera);
        // pixel at top left corner, and top left corner of the pixel
        Ray ray1 = tracer.FireRayAtPixel(0, 0, 0f, 0f);
        Assert.True(Point._ArePointsClose(new Point(0f, 4f, 2f), ray1.At(1.0f)));

        // top right corner of the pixel
        ray1 = tracer.FireRayAtPixel(0, 0, 1f, 0f);
        Assert.True(Point._ArePointsClose(new Point(0f, 2f, 2f), ray1.At(1.0f)));

        // bottom left corner of the pixel
        ray1 = tracer.FireRayAtPixel(0, 0, 0f, 1f);
        Assert.True(Point._ArePointsClose(new Point(0f, 4f, 0f), ray1.At(1.0f)));

        // bottom right corner of the pixel
        ray1 = tracer.FireRayAtPixel(0, 0, 1f, 1f);
        Assert.True(Point._ArePointsClose(new Point(0f, 2f, 0f), ray1.At(1.0f)));

        // We assign unusual values for uPixel and vPixel although these values are never reached 
        Ray ray2 = tracer.FireRayAtPixel(0, 0, 2.5f, 1.5f);
        Ray ray3 = tracer.FireRayAtPixel(2, 1, 0.5f, 0.5f);
        Assert.True(Ray._AreRaysClose(ray2, ray3));
        Assert.True(Point._ArePointsClose(new Point(0, -1.0f, -1.0f ), ray2.At(1.0f)));

        ray2 = tracer.FireRayAtPixel(0, 0, 0, 1);
        ray3 = tracer.FireRayAtPixel(0, 1, 0, 0);
        Assert.True(Ray._AreRaysClose(ray2, ray3));
        Assert.True(Point._ArePointsClose(new Point(0, 4, 0 ), ray2.At(1.0f)));

        ray2 = tracer.FireRayAtPixel(3, 0, 1, 1);
        ray3 = tracer.FireRayAtPixel(0, 1, 4, 0);
        Assert.True(Ray._AreRaysClose(ray2, ray3));
        Assert.True(Point._ArePointsClose(new Point(0, -4, 0 ), ray2.At(1.0f)));
    }
    
    [Fact]
    public void TestFireAllRays()
    {
        image = new HDRImage(4, 7);
        camera = new OrthogonalCamera(new Transformation(), 4/7.0f, 1);
        // each pixel has dimension 2×2
        tracer = new ImageTracer(image, camera, null, 5);
        Color expectedColor = new Color(1, 2, 3);
        tracer.FireAllRays(ray => expectedColor);
        for (int col = 0; col < image.Width; col++)
        {
            for (int row = 0; row < image.Height; row++)
            {
                Assert.Equal(expectedColor, image[col, row]);
            }
        }
    }
}