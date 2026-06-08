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

        Ray ray1 = tracer.FireRay(0, 1, 2.5f, 1.5f);
        Ray ray2 = tracer.FireRay(2, 0, 0.5f, 0.5f);
        Assert.True(Ray._AreRaysClose(ray1, ray2));

        ray1 = tracer.FireRay(0, 0, 0, 1);
        ray2 = tracer.FireRay(0, 1, 0, 2);
        Assert.True(Ray._AreRaysClose(ray1, ray2));

        ray1 = tracer.FireRay(3, 0, 1, 1);
        ray2 = tracer.FireRay(0, 1, 4, 2);
        Assert.True(Ray._AreRaysClose(ray1, ray2));
    }

    [Fact]
    public void Test_uv_mapping()
    {
        Ray ray = tracer.FireRay(0, 0, 0, 1);
        Point expectedPoint = new Point(0, camera.AspectRatio, 1); // top left of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));

        ray = tracer.FireRay(3, 0, 1, 1);
        expectedPoint = new Point(0, -camera.AspectRatio, 1); // top right of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));

        ray = tracer.FireRay(0, 1, 0, 0);
        expectedPoint = new Point(0, camera.AspectRatio, -1); // bottom left of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));

        ray = tracer.FireRay(3, 1, 1, 0);
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