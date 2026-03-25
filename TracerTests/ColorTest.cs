using System;
using Xunit;
using Colors;

namespace Trace.Test; //Per runnare un test occorre solo premere il triangolo verde accanto ad ogni singolo test (oppure accanto alla classe test per farli tutti)

public class ColorTest
{
    [Fact]
    public void TestSum()
    {
        var a = new Color(1.0f, 2.0f, 3.0f);
        var b = new Color(5.0f, 6.0f, 7.0f);
        Assert.True(Color._are_close(new Color(6.0f, 8.0f, 10.0f), a+b));
    }

    [Fact]
    public void TestProd_Color()
    {
        var a = new Color(1.0f, 2.0f, 3.0f);
        var b = new Color(5.0f, 6.0f, 7.0f);
        Assert.True(Color._are_close(new Color(5.0f, 12.0f, 21.0f), a*b));
    }

    [Fact]
    public void TestProd_Scalar()
    {
        var a = new Color(5.0f, 6.0f, 7.0f);
        const float c = 3f;
        Assert.True(Color._are_close(new Color(15.0f, 18.0f, 21.0f), a*c));
        Assert.True(Color._are_close(new Color(15.0f, 18.0f, 21.0f), c*a));
    }

    [Fact]
    public void Test_Luminosity()
    {
        var a = new Color(5.0f, 6.0f, 7.0f);
        Assert.Equal(6.0f, a.Luminosity());
    }
}