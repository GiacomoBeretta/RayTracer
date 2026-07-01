using TracerLib;

namespace TracerTests;
//Per avviare un test occorre solo premere il triangolo verde accanto a ogni singolo test (oppure accanto alla classe test per farli tutti)

public class ColorTest
{
    [Fact]
    public void TestConstructor()
    {
        Color a = new Color(1.0f, 24.0f, 192.8f);
        Assert.Equal(1.0f, a.R);
        Assert.Equal(24.0f, a.G);
        Assert.Equal(192.8f, a.B);

        Assert.Throws<ArgumentOutOfRangeException>(() => new Color(-2, 24.0f, 192.8f));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Color(289.0f, -0.4f, 29.3f));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Color(0, 289.0f, -1));
    }

    [Fact]
    public void TestSum()
    {
        Color c1 = new Color(1, 5, 18);
        Color c2 = new Color(509, 24, 627);
        Assert.Equal(new Color(510, 29, 645), c1 + c2);

        Color a = new Color(1.0f, 2.0f, 3.0f);
        Color b = new Color(5.0f, 6.0f, 7.0f);
        Assert.True(Color._AreColorsClose(new Color(6.0f, 8.0f, 10.0f), a + b));
    }

    [Fact]
    public void TestProductWithFloat()
    {
        Color c1 = new Color(1, 22, 333);
        float a = 2f;
        float b = 3.1f;
        Assert.Equal(new Color(2, 44, 666), c1 * a);
        (c1 * b).Print();
        Assert.True(Color._AreColorsClose(new Color(3.1f, 68.2f, 1032.2999f), c1 * b));

        Assert.Equal(c1 * a, a * c1);
        Assert.True(Color._AreColorsClose(c1 * b, b * c1));

        Color c2 = new Color(5.0f, 6.0f, 7.0f);
        const float c = 3f;
        Assert.True(Color._AreColorsClose(new Color(15.0f, 18.0f, 21.0f), c2 * c));
        Assert.True(Color._AreColorsClose(new Color(15.0f, 18.0f, 21.0f), c * c2));

        Assert.Throws<ArgumentOutOfRangeException>(() => c1 * -1);
    }

    [Fact]
    public void TestHadamardProduct()
    {
        Color c1 = new Color(0.5f, 23.7f, 480);
        Color c2 = new Color(873, 94.3f, 3.7f);
        Assert.True(Color._AreColorsClose(new Color(436.5f, 2234.9102f, 1776), c1 * c2));

        Color a = new Color(1.0f, 2.0f, 3.0f);
        Color b = new Color(5.0f, 6.0f, 7.0f);
        Assert.True(Color._AreColorsClose(new Color(5.0f, 12.0f, 21.0f), a * b));
    }

    [Fact]
    public void TestAreSameColor()
    {
        Color c1 = new Color(423.3f, 5, 18.8f);
        Color c2 = new Color(423.3f, 5, 18.8f);
        Assert.True(Color._AreSameColor(c1, c2));

        Color c3 = new Color(423.3f, 5, 18.800001f);
        Assert.False(Color._AreSameColor(c1, c3));
    }

    [Fact]
    public void TestAreColorsClose()
    {
        Color c1 = new Color(423.37737f, 5, 18.81416f);

        Color c2 = new Color(423.37738f, 5, 18.81415f);
        Assert.True(Color._AreColorsClose(c1, c2));

        Color c3 = new Color(423.3f, 5, 18.815f);
        Color c4 = new Color(423.3778f, 5.1f, 18.815f);
        Color c5 = new Color(423.3779f, 5, 18.8f);
        Assert.False(Color._AreColorsClose(c1, c3));
        Assert.False(Color._AreColorsClose(c1, c4));
        Assert.False(Color._AreColorsClose(c1, c5));
    }

    [Fact]
    public void TestToString()
    {
        Color a = new Color(2, 9, 5);
        Assert.Equal("(R=2, G=9, B=5)", a.ToString());
    }

    [Fact]
    public void TestLuminosityShirleyMorley()
    {
        Color a = new Color(5.0f, 6.2f, 7.0f);
        Assert.Equal(6.0f, a.LuminosityShirleyMorley()); //forse è meglio usare la funzione are close per i float?
    }

    [Fact]
    public void TestLuminosityWeightedAverage()
    {
        Color a = new Color(41.0f, 65.7f, 23.83f);
        //41.0*0.2126 + 65.7*0.7152 + 23.83*0.0722 = 8.7166 + 46.98864 + 1.720526 = 57.425766
        Assert.True(Functions.AreClose(57.42576f, a.LuminosityWeightedAverage()));
    }

    [Fact]
    public void TestClamp()
    {
        Color c1 = new Color(1.0f, 292.4f, 0);
        c1._Clamp();
        Color c2 = new Color(0.5f, 0.99659f, 0);
        Assert.True(Color._AreColorsClose(c2, c1));
    }

    [Fact]
    public void TestTo8BitRGB()
    {
        float gamma = 2;
        Color a = new Color(0.3726f, 0.472f, 0.2204f);
        Assert.Equal(new Color(156, 175, 120), a.To8BitRGB(gamma));

        Assert.Throws<ArgumentOutOfRangeException>(() => a.To8BitRGB(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => a.To8BitRGB(-1));
    }
}