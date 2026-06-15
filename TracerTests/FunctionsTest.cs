using TracerLib;

namespace TracerTests;

public class FunctionsTest
{
    [Fact]
    public void TestAreClose()
    {
        float a = 4.687908f;
        float b = 4.687901f;
        float c = 4.68791f;
        Assert.True(Functions.AreClose(a, b));
        Assert.True(Functions.AreClose(a, c));

        float d = 4.687918f;
        Assert.False(Functions.AreClose(a, d));

        float e = 4.6876f;
        Assert.False(Functions.AreClose(a, e, 1e-4f));
        float f = 4.68799f;
        Assert.True(Functions.AreClose(a, f, 1e-4f));
    }

    //da verificare che lanci l'eccezione
    [Fact]
    public void TestAreArrayClose()
    {
        float[] a = [2, 5.352f, 933, -39.6f];
        float[] b = [2, 5.355f, 933, -39.69f];

        Assert.False(Functions.AreArraysClose(a, b));
        Assert.True(Functions.AreArraysClose(a, b, 1e-1f));
    }

    [Fact]
    public void TestDegToRad()
    {
        float deg = 180f;
        
        Assert.True(Functions.AreClose(Functions.DegToRad(deg), MathF.PI));
        Assert.False(Functions.AreClose(Functions.DegToRad(deg), 64f));
    }

    [Fact]
    public void TestDegToRad()
    {
        var deg = 180f;
        
        Assert.True(Functions.AreClose(Functions.DegToRad(deg), MathF.PI));
        Assert.False(Functions.AreClose(Functions.DegToRad(deg), 64f));
    }
}