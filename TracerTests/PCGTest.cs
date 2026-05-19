using System.Globalization;
using TracerLib;
using Xunit.Abstractions;

namespace TracerTests;

public class PCGTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PCGTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void RandomTest()
    {
        var pcg = new PCG();
        
        Assert.Equal(1753877967969059832UL, pcg.State);
        Assert.Equal(109UL, pcg.Inc);
        
        uint[] value = [2707161783, 2068313097, 3122475824, 2211639955, 3215226955, 3421331566];

        foreach (var i in value)
        {
            Assert.Equal(pcg.Random(), i);
        }
    }

    [Fact]
    public void RandomFloatTest()
    {
        var pcg = new PCG();

        for (var i = 0; i < 10; i++)
        {
            var rand = pcg.RandomFloat();
            Assert.True(rand is < 1 and >= 0);
        }
    }
}