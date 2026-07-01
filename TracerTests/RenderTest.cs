using TracerLib;
using Xunit.Abstractions;

namespace TracerTests;

public class RenderTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public RenderTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void OnOffTest()
    {
        Sphere sphere = new Sphere(new Transformation(new Vector(2.0f, 0f, 0f)) * new Transformation(0.2f, 0.2f, 0.2f),
            new Material(new UniformPigment(new Color(1.0f, 1.0f, 1.0f)), new DiffuseBRDF()));

        HDRImage image = new HDRImage(3, 3);

        Camera camera = new OrthogonalCamera();

        ImageTracer tracer = new ImageTracer(image, camera);

        World world = new World();
        world.Add(sphere);

        Renderer render = new OnOffRenderer(world);

        tracer.FireAllRays(ray => render.RenderFunction(ray));

        Assert.True(Color._AreColorsClose(new Color(0f, 0f, 0f), image[0]));
        Assert.True(Color._AreColorsClose(new Color(0f, 0f, 0f), image[1]));
        Assert.True(Color._AreColorsClose(new Color(0f, 0f, 0f), image[2]));
        Assert.True(Color._AreColorsClose(new Color(0f, 0f, 0f), image[3]));
        Assert.True(Color._AreColorsClose(new Color(1f, 1f, 1f), image[4]));
        Assert.True(Color._AreColorsClose(new Color(0f, 0f, 0f), image[5]));
        Assert.True(Color._AreColorsClose(new Color(0f, 0f, 0f), image[6]));
        Assert.True(Color._AreColorsClose(new Color(0f, 0f, 0f), image[7]));
        Assert.True(Color._AreColorsClose(new Color(0f, 0f, 0f), image[8]));
    }

    [Fact]
    public void FlatTest()
    {
        Color sphereColor = new Color(1.0f, 2.0f, 3.0f);

        Transformation sphereTransformation =
            new Transformation(new Vector(2.0f, 0f, 0f)) * new Transformation(0.2f, 0.2f, 0.2f);
        Material sphereMaterial = new Material(new UniformPigment(sphereColor), new DiffuseBRDF());
        Sphere sphere = new Sphere(sphereTransformation, sphereMaterial);

        HDRImage image = new HDRImage(3, 3);

        Camera camera = new OrthogonalCamera();

        ImageTracer tracer = new ImageTracer(image, camera);

        World world = new World();
        world.Add(sphere);

        Renderer render = new FlatRenderer(world);

        tracer.FireAllRays(ray => render.RenderFunction(ray));

        /*for (int i = 0; i < 9; i++)
        {
            _testOutputHelper.WriteLine(image[i].ToString());
        }*/

        Color black = new Color(0f, 0f, 0f);
        Assert.True(Color._AreColorsClose(black, image[0, 0]));
        Assert.True(Color._AreColorsClose(black, image[1, 0]));
        Assert.True(Color._AreColorsClose(black, image[2, 0]));
        Assert.True(Color._AreColorsClose(black, image[0, 1]));
        Assert.True(Color._AreColorsClose(sphereColor, image[1, 1]));
        Assert.True(Color._AreColorsClose(black, image[2, 1]));
        Assert.True(Color._AreColorsClose(black, image[0, 2]));
        Assert.True(Color._AreColorsClose(black, image[1, 2]));
        Assert.True(Color._AreColorsClose(black, image[2, 2]));
    }

    //Test della fornace
    [Fact]
    public void PathTracingTest()
    {
        PCG pcg = new PCG();

        for (int i = 0; i < 5; i++)
        {
            float emittedRadiance = pcg.RandomFloat();
            float reflectance = pcg.RandomFloat() * 0.9f;

            World world = new World();

            Material enclosureMaterial = new Material(new UniformPigment(new Color(1.0f, 1.0f, 1.0f) * emittedRadiance),
                new DiffuseBRDF(new UniformPigment(new Color(1.0f, 1.0f, 1.0f) * reflectance)));

            world.Add(new Sphere(new Transformation(), enclosureMaterial));

            Renderer pathTracer = new PathTracingRenderer(world: world, pcg: pcg, numRay: 1,
                russianRouletteStartDepth: 101, maxDepth: 100);

            Ray ray = new Ray(new Point(0f, 0f, 0f), new Vector(1f, 0f, 0f));
            Color color = pathTracer.RenderFunction(ray);

            float expected = emittedRadiance / (1.0f - reflectance);

            Assert.True(Functions.AreClose(expected, color.R, 1e-3f));
            Assert.True(Functions.AreClose(expected, color.G, 1e-3f));
            Assert.True(Functions.AreClose(expected, color.B, 1e-3f));
        }
    }
}