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
        PCG pcg = new PCG();
        
        Assert.Equal(1753877967969059832UL, pcg.State);
        Assert.Equal(109UL, pcg.Inc);
        
        uint[] value = [2707161783, 2068313097, 3122475824, 2211639955, 3215226955, 3421331566];

        foreach (uint i in value)
        {
            Assert.Equal(pcg.Random(), i);
        }
    }

    [Fact]
    public void RandomFloatTest()
    {
        PCG pcg = new PCG();

        float[] value =
        [
            0.6303102204110473f, 0.4815666696522385f, 0.7270080558955669f,
            0.5149375542532653f, 0.7486033614259213f, 0.7965908306650817f
        ];
        
        foreach (float i in value)
        {
            Assert.Equal(pcg.RandomFloat(), i);
        }
        
        for (int i = 0; i < 10; i++)
        {
            Assert.True(pcg.RandomFloat() is < 1 and >= 0);
        }
    }
}