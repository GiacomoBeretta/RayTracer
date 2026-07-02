using TracerLib;

namespace TracerTests;

public class NormalTest
{
    [Fact]
    public void NegationTest()
    {
        Normal n = new Normal(1.0f, 2.0f, 3.0f);

        Assert.True(Normal._AreNormalsClose(new Normal(-1.0f, -2.0f, -3.0f), -n));
        // La funzione Equal non funziona (Ricotrollare arrotondamento)
        // Assert.Equal(new Normal(-1.0f, -2.0f, -3.0f), n._NormalNegation());
    }

/*
    [Fact]
    public void ScalarProductTest()
    {
        Normal n = new Normal(1.0f, 2.0f, 3.0f);
        float a = 5.0f;

        Assert.True(Normal._AreNormalsClose(new Normal(5.0f, 10.0f, 15.0f), n * a));
        Assert.True(Normal._AreNormalsClose(new Normal(5.0f, 10.0f, 15.0f), a * n));
    }
*/

    [Fact]
    public void NormalVectorProductTest()
    {
        Normal n = new Normal(1.0f, 2.0f, 3.0f);
        Vector v = new Vector(4.0f, 5.0f, 6.0f);

        Assert.True(Functions.AreClose(32.0f, n * v));
        Assert.True(Functions.AreClose(32.0f, v * n));
    }

    [Fact]
    public void NormalVectorCrossTest()
    {
        Normal n = new Normal(1.0f, 2.0f, 3.0f);
        Vector v = new Vector(4.0f, 5.0f, 6.0f);
        Normal n2 = new Normal(7.0f, 8.0f, 9.0f);

        Assert.True(Vector._AreVectorsClose(new Vector(-3.0f, 6f, -3f), Normal.CrossProduct(n, v)));
        Assert.True(Vector._AreVectorsClose(new Vector(3.0f, -6.0f, 3.0f), Normal.CrossProduct(v, n)));
        Assert.True(Vector._AreVectorsClose(new Vector(6.0f, -12.0f, 6.0f), Normal.CrossProduct(n2, n)));
    }

    //Prima di proseguire con i test del proddoto vettore occorre implementare la _AreCloseVector
    [Fact]
    public void Norm()
    {
        Normal n = new Normal(2.0f, 3.0f, 6.0f);

        Assert.True(Functions.AreClose(49.0f, n.SquaredNorm()));
        Assert.True(Functions.AreClose(7.0f, n.Norm()));

        //Trovare un vettore che non restituisca una normale troppo complessa una vorlta normalizzata
        //Assert.True(Normal._AreCloseNormal(new Normal(0.29f, 0.43f, 0.86f), n.Normalize()));
    }

    [Fact]
    public void TestIsNormalized()
    {
        Normal n = new Normal(0f, 0f, 1f);
        
        Assert.True(n.IsNormalized());
    }

    [Fact]
    public void TestCheckNormalized()
    {
        Normal n = new Normal(2f, 0f, 0f);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            n.CheckNormalized();
        });
    }

    [Fact]
    public void TestToVector()
    {
        Normal n = new Normal(1f, 2f, 3f);

        Vector v = n.ToVector();
        
        Assert.True(Vector._AreVectorsClose(new Vector(1f, 2f, 3f), v));
    }
}