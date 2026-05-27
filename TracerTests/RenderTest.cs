using TracerLib;

namespace TracerTests;

public class RenderTest
{
    [Fact]
    public void OnOffTest()
    {
        var sphere = new Sphere(new Transformation(new Vector(2.0f, 0f, 0f)) * new Transformation(0.2f, 0.2f, 0.2f), new Material(new UniformPigment(new Color(1.0f, 1.0f, 1.0f)), new DiffuseBRDF()));

        var image = new HDRImage(3, 3);

        ICamera camera = new OrthogonalCamera(); //RICONTROLLARE PERCHÈ IL COSTRUTTORE VUOTO CREA PROBLEMI

        var tracer = new ImageTracer(image, camera);

        var world = new World();
        world.Add(sphere);

        Render render = new OnOff(world);

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
}