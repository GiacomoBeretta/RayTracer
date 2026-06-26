using TracerLib;

namespace TracerTests;

public class BRDF_Test
{
    [Fact]
    public void ConstructorTest()
    {
        DiffuseBRDF diffuseBrdf = new DiffuseBRDF();
        Color black = new Color(0, 0, 0);
       
        Assert.Equal(typeof(UniformPigment), (diffuseBrdf.Pigment).GetType());
        Color pigmentColor = ((UniformPigment)diffuseBrdf.Pigment).Color;
        Assert.True(Color._AreColorsClose(black, pigmentColor));
    }
    
    [Fact]
    public void DiffuseScatterRay()
    {
        PCG pcg1 = new PCG();
        PCG pcg2 = new PCG();

        DiffuseBRDF diffuseBrdf = new DiffuseBRDF();

        Point intersect = new Point(0, 0, 0);
        Vector vin = new Vector(0, 0, -1);
        Normal n = new Normal(0,0,1);

        Vector e1 = new Vector(1, 0, 0);
        Vector e2 = new Vector(0, 1, 0);
        Vector e3 = new Vector(0, 0, 1);

        for (int i = 0; i < 10; i++)
        {
            Ray ray_out =
                diffuseBrdf.ScatterRay(pcg1, vin, intersect, n, 3);
            float phi = 2 * MathF.PI * pcg2.RandomFloat();
            float cos_theta_sq = pcg2.RandomFloat();
            float cos_theta = MathF.Sqrt(cos_theta_sq);
            float sin_theta = MathF.Sqrt(1 - cos_theta_sq);

            Ray expected = new Ray
            (
                new Point(0, 0, 0),
                e1 * sin_theta * MathF.Cos(phi) + e2 * sin_theta * MathF.Sin(phi) + e3 * cos_theta,
                1e-03f,
                float.PositiveInfinity,
                3
            );
            
            Assert.Equal(expected, ray_out);
        }
    }

    [Fact]
    public void SpecularScatterRay()
    {
        PCG pcg = new PCG();
        Vector vin = new Vector(0,0,-1);
        Point intersect = new Point(0, 0, 0);
        Normal n = new Normal(0,0,1);
        SpecularBRDF specularBrdf = new SpecularBRDF();
        
        Ray ray = specularBrdf.ScatterRay(pcg, vin, intersect, n, 0);
        Ray expected = new Ray(intersect, new Vector(0, 0, 1), tmin:1e-3f);
        Assert.Equal(expected, ray);
        vin = new Vector(-1, 1, 0);
        n = new Normal(1,0,0);
        ray = specularBrdf.ScatterRay(pcg, vin, intersect, n, 0);
        expected = new Ray(intersect, new Vector(1, 1, 0), tmin:1e-3f);
        Assert.Equal(expected, ray);

        vin = new Vector(1, 1, -1);
        n = new Normal(0,-1,0);
        ray = specularBrdf.ScatterRay(pcg, vin, intersect, n, 0);
        expected = new Ray(intersect, new Vector(1, -1, -1), tmin:1e-3f);
        Assert.Equal(expected, ray);

    }
}