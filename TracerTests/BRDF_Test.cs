using TracerLib;

namespace TracerTests;

public class DiffuseBRDF_Test
{
    [Fact]
    public void DiffuseBRDFTest()
    {
        PCG pcg = new PCG();

        for (int i = 0; i < 10; i++)
        {
            float theta = MathF.Acos(pcg.RandomFloat());
            Console.WriteLine("theta = " + theta);
        }
    }
}