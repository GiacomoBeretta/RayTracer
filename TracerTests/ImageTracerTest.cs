using TracerLib;

namespace TracerTests;

public class ImageTracerTest
{
    [Fact]
    public void TestFireRay()
    {
        HDRImage image = new HDRImage(4, 2);
        ICamera camera = new PerspectiveCamera(1, 2, new Transformation());
        ImageTracer tracer = new ImageTracer(image, camera);

        Ray ray1 = tracer.FireRay(0, 0, 2.5f, 1.5f);
        Ray ray2 = tracer.FireRay(2, 1, 0.5f, 0.5f);
        Assert.True(Ray._AreRaysClose(ray1, ray2));

        Color expected = new Color(1, 2, 3);
        tracer.FireAllRays(ray => expected);
        for (int col = 0; col < image.Width; col++)
        {
            for (int row = 0; row < image.Height; row++)
            {
                Assert.Equal(expected, image[col, row]);
            }
        }
    }
}