using Xunit;
using TracerLib;

namespace TracerTests;

public class ColorTest
{
    [Fact]
    public void TestAdd()
    {
        Color c1 = new Color(1, 5, 18);
        Color c2 = new Color(509, 24, 627);
        Assert.Equal(new Color(510, 29, 645), c1 + c2);
        //Assert.True(Color.AreSameColor(new Color(510, 29, 645), c1 + c2));
    }

    [Fact]
    public void TestScalarProduct()
    {
        Color c1 = new Color(1, 22, 333);
        float a = 2f;
        float b = 3.1f;
        Assert.Equal(new Color(2, 44, 666), c1 * a);
        //Assert.True(Color.AreSameColor(new Color(2, 44, 666), c1 * a));
        Assert.True(Color.AreCloseColor(new Color(3.1f, 68.2f, 1032.3f), c1 * b));

        Assert.Equal(c1 * a, a * c1);
        //Assert.True(Color.AreSameColor(c1 * a, a * c1));
        Assert.True(Color.AreCloseColor(c1 * b, b * c1));
    }

    [Fact]
    public void TestProduct()
    {
        Color c1 = new Color(0.5f, 23.7f, 480);
        Color c2 = new Color(873, 94.3f, 3.7f);
        Assert.True(Color.AreCloseColor(new Color(436.5f, 2234.91f, 1776), c1 * c2));
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
        Assert.True(Color.AreCloseColor(c1, c2));

        Color c3 = new Color(423.3f, 5, 18.815f);
        Color c4 = new Color(423.3778f, 5.1f, 18.815f);
        Color c5 = new Color(423.3779f, 5, 18.8f);
        Assert.False(Color.AreCloseColor(c1, c3));
        Assert.False(Color.AreCloseColor(c1, c4));
        Assert.False(Color.AreCloseColor(c1, c5));
    }
}