using TracerLib;
using Xunit.Abstractions;

namespace TracerTests;

public class PigmentTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private UniformPigment up;
    private CheckeredPigment cp;
    private ImagePigment ip;
    
    public PigmentTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        up = new UniformPigment(new Color(1.0f, 2.0f, 3.0f));
        cp = new CheckeredPigment(new Color(1.0f, 2.0f, 3.0f), new Color(10.0f, 20.0f, 30.0f), 2);
        ip = new ImagePigment(new HDRImage(2, 2));
    }
    
    [Fact]
    public void UniformConstructorTest()
    {
        Assert.True(Functions.AreClose(1.0f, up.Color.R));
        Assert.True(Functions.AreClose(2.0f, up.Color.G));
        Assert.True(Functions.AreClose(3.0f, up.Color.B));
        
        Assert.True(Color._AreColorsClose(new Color(1.0f, 2.0f, 3.0f), up.Color));
    }

    [Fact]
    public void UniformGetColorTest()
    {
        
        var v1 = new Vector2D(0.5f, 0.5f);
        var v2 = new Vector2D(1.0f, 1.0f);
        var v3 = new Vector2D(0.0f, 0.0f);
        var v4 = new Vector2D(0.0f, 1.0f);
        var v5 = new Vector2D(1.0f, 0.0f);
        
        Assert.True(Color._AreColorsClose(new Color(1.0f, 2.0f, 3.0f), up.GetColor(v1)));
        Assert.True(Color._AreColorsClose(new Color(1.0f, 2.0f, 3.0f), up.GetColor(v2)));
        Assert.True(Color._AreColorsClose(new Color(1.0f, 2.0f, 3.0f), up.GetColor(v3)));
        Assert.True(Color._AreColorsClose(new Color(1.0f, 2.0f, 3.0f), up.GetColor(v4)));
        Assert.True(Color._AreColorsClose(new Color(1.0f, 2.0f, 3.0f), up.GetColor(v5)));
    }
    
    //IMPLEMENTARE TEST PER IL COSTRUTTORE IMAGEPIGMENT

    [Fact]
    public void ImageGetColor()
    {
        ip.Image[0] = new Color(1.0f, 2.0f, 3.0f);
        ip.Image[1] = new Color(2.0f, 3.0f, 1.0f);
        ip.Image[2] = new Color(2.0f, 1.0f, 3.0f);
        ip.Image[3] = new Color(3.0f, 2.0f, 1.0f);
        
        Assert.True(Color._AreColorsClose(ip.GetColor(new Vector2D(0.0f, 0.0f)), new Color(1.0f, 2.0f, 3.0f)));
        Assert.True(Color._AreColorsClose(ip.GetColor(new Vector2D(1.0f, 0.0f)), new Color(2.0f, 3.0f, 1.0f)));
        Assert.True(Color._AreColorsClose(ip.GetColor(new Vector2D(0.0f, 1.0f)), new Color(2.0f, 1.0f, 3.0f)));
        Assert.True(Color._AreColorsClose(ip.GetColor(new Vector2D(1.0f, 1.0f)), new Color(3.0f, 2.0f, 1.0f)));
    }

    [Fact]
    public void CheckeredConstructorTest()
    {
        Assert.True(Functions.AreClose(1.0f, cp.Color1.R));
        Assert.True(Functions.AreClose(2.0f, cp.Color1.G));
        Assert.True(Functions.AreClose(3.0f, cp.Color1.B));
        
        Assert.True(Functions.AreClose(10.0f, cp.Color2.R));
        Assert.True(Functions.AreClose(20.0f, cp.Color2.G));
        Assert.True(Functions.AreClose(30.0f, cp.Color2.B));
        
        Assert.True(Color._AreColorsClose(new Color(1.0f, 2.0f, 3.0f), cp.Color1));
        Assert.True(Color._AreColorsClose(new Color(10.0f, 20.0f, 30.0f), cp.Color2));
        
        Assert.Equal(2, cp.NumSteps);
    }

    [Fact]
    public void CheckeredGetColor()
    {
        Assert.True(Color._AreColorsClose(cp.Color1, cp.GetColor(new Vector2D(0.25f, 0.25f))));
        Assert.True(Color._AreColorsClose(cp.Color2, cp.GetColor(new Vector2D(0.75f, 0.25f))));
        Assert.True(Color._AreColorsClose(cp.Color2, cp.GetColor(new Vector2D(0.25f, 0.75f))));
        Assert.True(Color._AreColorsClose(cp.Color1, cp.GetColor(new Vector2D(0.75f, 0.75f))));
    }
}

public class MaterialTest
{
    
}