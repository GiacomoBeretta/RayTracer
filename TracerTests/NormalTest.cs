using TracerLib;

namespace TracerTests;

public class NormalTest
{
    [Fact]
    public void NegationTest()
    {
        var n = new Normal(1.0f, 2.0f, 3.0f);
        
        Assert.True(Normal._AreCloseNormal(new Normal(-1.0f, -2.0f, -3.0f), n._NormalNegation())); 
        // La funzione Equal non funziona (Ricotrollare arrotondamento)
        // Assert.Equal(new Normal(-1.0f, -2.0f, -3.0f), n._NormalNegation());
    }

    [Fact]
    public void ScalarProductTest()
    {
        var n = new Normal(1.0f, 2.0f, 3.0f);
        var a = 5.0f;
        
        Assert.True(Normal._AreCloseNormal(new Normal(5.0f, 10.0f, 15.0f), n * a));
        Assert.True(Normal._AreCloseNormal(new Normal(5.0f, 10.0f, 15.0f), a * n));
    }

    [Fact]
    public void NormalVectorProduct()
    {
        var n = new Normal(1.0f, 2.0f, 3.0f);
        var v = new Vector(4.0f, 5.0f, 6.0f);
        
        Assert.True(Functions.AreClose(32.0f , n * v));
        Assert.True(Functions.AreClose(32.0f , v * n));
    }

    //Prima di proseguire con i test del proddoto vettore occorre implementare la _AreCloseVector
    [Fact]
    public void Norm()
    {
        var n = new Normal(2.0f, 3.0f, 6.0f);
        
        Assert.True(Functions.AreClose(49.0f, n.SquaredNorm()));
        Assert.True(Functions.AreClose(7.0f, n.Norm()));
        
        //Trovare un vettore che non restituisca una normale troppo complessa una vorlta normalizzata
        //Assert.True(Normal._AreCloseNormal(new Normal(0.29f, 0.43f, 0.86f), n.Normalize()));
    }
    
}