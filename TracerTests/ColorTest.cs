//using System;

using Xunit;
using TracerLib;

//using Colors;

namespace TracerTests;
//Per runnare un test occorre solo premere il triangolo verde accanto ad ogni singolo test (oppure accanto alla classe test per farli tutti)

public class ColorTest
{
    [Fact]
    public void TestSum()
    {
        Color c1 = new Color(1, 5, 18);
        Color c2 = new Color(509, 24, 627);
        Assert.Equal(new Color(510, 29, 645), c1 + c2);

        var a = new Color(1.0f, 2.0f, 3.0f);
        var b = new Color(5.0f, 6.0f, 7.0f);
        Assert.True(Color._AreCloseColor(new Color(6.0f, 8.0f, 10.0f), a + b));
    }


    [Fact]
    public void TestScalarProduct()
    {
        Color c1 = new Color(1, 22, 333);
        float a = 2f;
        float b = 3.1f;
        Assert.Equal(new Color(2, 44, 666), c1 * a);
        //Assert.True(Color.AreSameColor(new Color(2, 44, 666), c1 * a));
        Assert.True(Color._AreCloseColor(new Color(3.1f, 68.2f, 1032.3f), c1 * b));

        Assert.Equal(c1 * a, a * c1);
        //Assert.True(Color.AreSameColor(c1 * a, a * c1));
        Assert.True(Color._AreCloseColor(c1 * b, b * c1));

        var c2 = new Color(5.0f, 6.0f, 7.0f);
        const float c = 3f;
        Assert.True(Color._AreCloseColor(new Color(15.0f, 18.0f, 21.0f), c2 * c));
        Assert.True(Color._AreCloseColor(new Color(15.0f, 18.0f, 21.0f), c * c2));
    }

    [Fact]
    public void TestHadamardProduct()
    {
        Color c1 = new Color(0.5f, 23.7f, 480);
        Color c2 = new Color(873, 94.3f, 3.7f);
        Assert.True(Color._AreCloseColor(new Color(436.5f, 2234.91f, 1776), c1 * c2));

        var a = new Color(1.0f, 2.0f, 3.0f);
        var b = new Color(5.0f, 6.0f, 7.0f);
        Assert.True(Color._AreCloseColor(new Color(5.0f, 12.0f, 21.0f), a * b));
    }

    [Fact]
    public void TestSameColor()
    {
        Color c1 = new Color(423.3f, 5, 18.8f);
        Color c2 = new Color(423.3f, 5, 18.8f);
        Assert.Equal(c1, c2);
        //Assert.True(Color.AreSameColor(c1, c2));

        Color c3 = new Color(423.3f, 5, 18.800001f);
        Assert.NotEqual(c1, c3);
    }

    [Fact]
    public void TestCloseColor()
    {
        Color c1 = new Color(423.3779f, 5, 18.8141f);

        Color c2 = new Color(423.3778f, 5, 18.815f);
        Assert.True(Color._AreCloseColor(c1, c2));

        Color c3 = new Color(423.3f, 5, 18.815f);
        Color c4 = new Color(423.3778f, 5.1f, 18.815f);
        Color c5 = new Color(423.3779f, 5, 18.8f);
        Assert.False(Color._AreCloseColor(c1, c3));
        Assert.False(Color._AreCloseColor(c1, c4));
        Assert.False(Color._AreCloseColor(c1, c5));
    }

    [Fact]
    public void Test_Luminosity()
    {
        var a = new Color(5.0f, 6.0f, 7.0f);
        Assert.Equal(6.0f, a.Luminosity());
    }
}