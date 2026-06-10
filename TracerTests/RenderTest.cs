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
        var sphere = new Sphere(new Transformation(new Vector(2.0f, 0f, 0f)) * new Transformation(0.2f, 0.2f, 0.2f), new Material(new UniformPigment(new Color(1.0f, 1.0f, 1.0f)), new DiffuseBRDF()));

        var image = new HDRImage(3, 3);

        ICamera camera = new OrthogonalCamera(); 

        var tracer = new ImageTracer(image, camera);

        var world = new World();
        world.Add(sphere);

        Renderer render = new OnOffRenderer(world);

        tracer.FireAllRays(ray => render.RenderFunction(ray));
        
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[0]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[1]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[2]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[3]));
        Assert.True(Color._AreColorsClose(new Color(1f,1f,1f), image[4]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[5]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[6]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[7]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[8])); 
    }
    
    [Fact] 
    public void FlatTest()
    {
        var sphereColor = new Color(1.0f, 2.0f, 3.0f);
        
        var sphere = new Sphere(new Transformation(new Vector(2.0f, 0f, 0f)) * new Transformation(0.2f, 0.2f, 0.2f), new Material(new UniformPigment(sphereColor), new DiffuseBRDF()));

        var image = new HDRImage(3, 3);

        ICamera camera = new OrthogonalCamera(); 

        var tracer = new ImageTracer(image, camera);

        var world = new World();
        world.Add(sphere);

        Renderer render = new FlatRenderer(world);

        tracer.FireAllRays(ray => render.RenderFunction(ray));
        
        _testOutputHelper.WriteLine(image[1].ToString());
        
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[0]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[1]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[2]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[3]));
        Assert.True(Color._AreColorsClose(sphereColor, image[4]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[5]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[6]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[7]));
        Assert.True(Color._AreColorsClose(new Color(0f,0f,0f), image[8]));
    }

    //Test della fornace
    [Fact]
    public void PathTracingTest()
    {
        var pcg = new PCG();

        for (var i = 0; i < 5; i++)
        {
            var emittedRadiance = pcg.RandomFloat();
            var reflectance = pcg.RandomFloat() * 0.9f;

            var world = new World();

            var enclosureMaterial = new Material(new UniformPigment(new Color(1.0f, 1.0f, 1.0f) * emittedRadiance),
                new DiffuseBRDF(new UniformPigment(new Color(1.0f, 1.0f, 1.0f) * reflectance)));
            
            world.Add(new Sphere(new Transformation(), enclosureMaterial));

            Renderer pathTracer = new PathTracingRenderer(world: world, pcg: pcg, numRay: 1, russianRouletteStartDepth: 101, maxDepth: 100);

            var ray = new Ray(new Point(0f, 0f, 0f), new Vector(1f, 0f, 0f));
            var color = pathTracer.RenderFunction(ray);

            var expected = emittedRadiance / (1.0f - reflectance);
            
            Assert.True(Functions.AreClose(expected, color.R, 1e-3f));
            Assert.True(Functions.AreClose(expected, color.G, 1e-3f));
            Assert.True(Functions.AreClose(expected, color.B, 1e-3f));
        }
    }
}