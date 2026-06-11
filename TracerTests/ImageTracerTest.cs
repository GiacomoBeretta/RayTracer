using TracerLib;
using Xunit.Abstractions;

namespace TracerTests;

public class ImageTracerTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private HDRImage image;
    private ICamera camera;
    private ImageTracer tracer;

    public ImageTracerTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        image = new HDRImage(4, 2);
        camera = new PerspectiveCamera(new Transformation(), 1f, 2f);
        tracer = new ImageTracer(image, camera);
    }

    [Fact]
    public void TestFireRay_uv_PixelSubmapping()
    {
        //ImageTracerTest test = new ImageTracerTest();

        // pixel at top left corner, and top left corner of the pixel
        Ray ray1 = tracer.FireRayAtPixel(0, 0, 0f, 0f); 
        Assert.True(Point._ArePointsClose(new Point(0f, 2f, 1f), ray1.At(1.0f)));

        // top right corner of the pixel
        ray1 = tracer.FireRayAtPixel(0, 0, 1f, 0f);
        Assert.True(Point._ArePointsClose(new Point(0f, 1f, 1f), ray1.At(1.0f)));
        
        // bottom left corner of the pixel
        ray1 = tracer.FireRayAtPixel(0, 0, 0f, 1f);
        Assert.True(Point._ArePointsClose(new Point(0f, 2f, 0f), ray1.At(1.0f)));
        
        // bottom right corner of the pixel
        ray1 = tracer.FireRayAtPixel(0, 0, 1f, 1f);
        Assert.True(Point._ArePointsClose(new Point(0f, 1f, 0f), ray1.At(1.0f)));
        
        // We assign unusual values for uPixel and vPixel although these values are never reached 
        Ray ray2 = tracer.FireRayAtPixel(0, 0, 2.5f, 1.5f);
        Ray ray3 = tracer.FireRayAtPixel(2, 1, 0.5f, 0.5f);
        Assert.True(Ray._AreRaysClose(ray2, ray3));

        ray2 = tracer.FireRayAtPixel(0, 0, 0, 1);
        ray3 = tracer.FireRayAtPixel(0, 1, 0, 0);
        Assert.True(Ray._AreRaysClose(ray2, ray3));

        ray2 = tracer.FireRayAtPixel(3, 0, 1, 1);
        ray3 = tracer.FireRayAtPixel(0, 1, 4, 0);
        Assert.True(Ray._AreRaysClose(ray2, ray3));
    }

    [Fact]
    public void Test_uv_mapping()
    {
        Ray ray = tracer.FireRayAtPixel(0, 0, 0, 0);
        Point expectedPoint = new Point(0, camera.AspectRatio, 1f); // top left of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));

        ray = tracer.FireRayAtPixel(3, 0, 1, 0);
        expectedPoint = new Point(0, -camera.AspectRatio, 1); // top right of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));

        ray = tracer.FireRayAtPixel(0, 1, 0, 1);
        expectedPoint = new Point(0, camera.AspectRatio, -1); // bottom left of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));

        ray = tracer.FireRayAtPixel(3, 1, 1, 1);
        expectedPoint = new Point(0, -camera.AspectRatio, -1); // bottom right of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));
    }

    [Fact]
    public void TestFireAllRays()
    {
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