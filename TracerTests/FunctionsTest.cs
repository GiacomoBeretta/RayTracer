using TracerLib;

namespace TracerTests;

public class FunctionsTest
{
    [Fact]
    public void TestEnsureGreaterThan()
    {
        int a = 4;
        int b = 12;
        Assert.Throws<ArgumentOutOfRangeException>(() => Functions.EnsureGreaterThan(a, nameof(a), b));

        a = 0;
        b = 0;
        Assert.Throws<ArgumentOutOfRangeException>(() => Functions.EnsureGreaterThan(a, nameof(a), b));
        
        float c = 4.00001f;
        float d = 4.00009f;
        Assert.Throws<ArgumentOutOfRangeException>(() => Functions.EnsureGreaterThan(c, nameof(c), d));

        c = 4.00001f;
        d = 4.00001f;
        Assert.Throws<ArgumentOutOfRangeException>(() => Functions.EnsureGreaterThan(c, nameof(c), d));
    }

    [Fact]
    public void TestEnsureGreaterThanOrEqual()
    {
        int a = 4;
        int b = 12;
        Assert.Throws<ArgumentOutOfRangeException>(() => Functions.EnsureGreaterThanOrEqual(a, nameof(a), b));
        
        float c = 4.00001f;
        float d = 4.00009f;
        Assert.Throws<ArgumentOutOfRangeException>(() => Functions.EnsureGreaterThanOrEqual(c, nameof(c), d));
    }

    [Fact]
    public void TestEnsureInRange()
    {
        int a = -3;
        int min = -2;
        int max = 2;
        Assert.Throws<ArgumentOutOfRangeException>(() => Functions.EnsureInRange(a, nameof(a), min, max));

        float b = 3.19f;
        float minf = 3.2f;
        float maxf = 3.4f;
        Assert.Throws<ArgumentOutOfRangeException>(() => Functions.EnsureInRange(b, nameof(b), minf, maxf));
    }
    
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
    public void VariableTableTest()
    {
        string[] variable1 = ["clock:150"];
        string[] variable2 = ["color:3:2"];
        string[] variable3 = ["hello:world"];

        var result = Functions.ParseVariableTable(variable1);

        Assert.Contains("clock", result.Keys);
        Assert.Equal(150, result["clock"]);

        Assert.Throws<ArgumentException>(() => Functions.ParseVariableTable(variable2));
        Assert.Throws<ArgumentException>(() => Functions.ParseVariableTable(variable3));
    }
}