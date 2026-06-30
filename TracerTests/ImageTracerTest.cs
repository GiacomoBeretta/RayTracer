using TracerLib;

namespace TracerTests;

public class ImageTracerTest
{
    public required HDRImage Image;
    public required Camera Cam;
    public required ImageTracer Tracer;
    
    [Fact]
    public void TestChecksInConstructor()
    {
        Image = new HDRImage(4, 2);
        Cam = new PerspectiveCamera(new Transformation(),2,1);
        Tracer = new ImageTracer(Image, Cam);
        
        Cam = new PerspectiveCamera(new Transformation(), 1, 0.5f, 2);
        Tracer = new ImageTracer(Image, Cam);
        
        // assert that a negative pixel Side Subdivisions is not permitted
        Assert.Throws<ArgumentOutOfRangeException>(() => new ImageTracer(Image, Cam, null, 0));
        
        // assert that image plane and pixel grid must not have different aspect ratios.
        Cam = new PerspectiveCamera(new Transformation(), 2, 2, 2);
        Assert.Throws<ArgumentOutOfRangeException>(() => new ImageTracer(Image, Cam));
    }
    
    [Fact]
    public void Test_uv_mapping()
    {
        Image = new HDRImage(4, 2);
        Cam = new PerspectiveCamera(new Transformation(), 8, 4, 2);
        Tracer = new ImageTracer(Image, Cam);
        Ray ray = Tracer.FireRayAtPixel(0, 0, 0, 0);
        Point expectedPoint = new Point(0, Cam.Width/2, Cam.Height/2); // top left of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));

        ray = Tracer.FireRayAtPixel(3, 0, 1, 0);
        expectedPoint = new Point(0, -Cam.Width/2, Cam.Height/2); // top right of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));

        ray = Tracer.FireRayAtPixel(0, 1, 0, 1);
        expectedPoint = new Point(0, Cam.Width/2, -Cam.Height/2); // bottom left of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));

        ray = Tracer.FireRayAtPixel(3, 1, 1, 1);
        expectedPoint = new Point(0, -Cam.Width/2, -Cam.Height/2); // bottom right of the screen
        Assert.True(Point._ArePointsClose(expectedPoint, ray.At(1)));
    }

    [Fact]
    public void TestFireRay_uv_PixelSubmapping()
    {
        Image = new HDRImage(4, 2);
        Cam = new PerspectiveCamera(new Transformation(), 8, 4, 2);
        // each pixel has dimension 2×2
        Tracer = new ImageTracer(Image, Cam);
        // pixel at top left corner, and top left corner of the pixel
        Ray ray1 = Tracer.FireRayAtPixel(0, 0, 0f, 0f);
        Assert.True(Point._ArePointsClose(new Point(0f, 4f, 2f), ray1.At(1.0f)));

        // top right corner of the pixel
        ray1 = Tracer.FireRayAtPixel(0, 0, 1f, 0f);
        Assert.True(Point._ArePointsClose(new Point(0f, 2f, 2f), ray1.At(1.0f)));

        // bottom left corner of the pixel
        ray1 = Tracer.FireRayAtPixel(0, 0, 0f, 1f);
        Assert.True(Point._ArePointsClose(new Point(0f, 4f, 0f), ray1.At(1.0f)));

        // bottom right corner of the pixel
        ray1 = Tracer.FireRayAtPixel(0, 0, 1f, 1f);
        Assert.True(Point._ArePointsClose(new Point(0f, 2f, 0f), ray1.At(1.0f)));

        // We assign unusual values for uPixel and vPixel although these values are never reached 
        Ray ray2 = Tracer.FireRayAtPixel(0, 0, 2.5f, 1.5f);
        Ray ray3 = Tracer.FireRayAtPixel(2, 1, 0.5f, 0.5f);
        Assert.True(Ray._AreRaysClose(ray2, ray3));
        Assert.True(Point._ArePointsClose(new Point(0, -1.0f, -1.0f ), ray2.At(1.0f)));

        ray2 = Tracer.FireRayAtPixel(0, 0, 0, 1);
        ray3 = Tracer.FireRayAtPixel(0, 1, 0, 0);
        Assert.True(Ray._AreRaysClose(ray2, ray3));
        Assert.True(Point._ArePointsClose(new Point(0, 4, 0 ), ray2.At(1.0f)));

        ray2 = Tracer.FireRayAtPixel(3, 0, 1, 1);
        ray3 = Tracer.FireRayAtPixel(0, 1, 4, 0);
        Assert.True(Ray._AreRaysClose(ray2, ray3));
        Assert.True(Point._ArePointsClose(new Point(0, -4, 0 ), ray2.At(1.0f)));
    }
    
    [Fact]
    public void TestFireAllRays()
    {
        Image = new HDRImage(4, 7);
        Cam = new OrthogonalCamera(new Transformation(), 4/7.0f, 1);
        // each pixel has dimension 2×2
        Tracer = new ImageTracer(Image, Cam, null, 5);
        Color expectedColor = new Color(1, 2, 3);
        Tracer.FireAllRays(ray => expectedColor);
        for (int col = 0; col < Image.Width; col++)
        {
            for (int row = 0; row < Image.Height; row++)
            {
                Assert.Equal(expectedColor, Image[col, row]);
            }
        }
    }
}