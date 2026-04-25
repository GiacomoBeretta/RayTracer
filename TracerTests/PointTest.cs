using TracerLib;

namespace TracerTests;

public class PointTest
{
    [Fact]
    public void TestPointVectorSum()
    {
        var p = new Point(1.0f, 2.0f, 3.0f);
        var v = new Vector(101.0f, 102.0f, 103.0f);
        
        Assert.Equal(new Point(102.0f, 104.0f, 106.0f), p + v);
        Assert.Equal(new Point(102.0f, 104.0f, 106.0f), v + p);
        
        Assert.True(Point._ArePointsClose(new Point(102.0f, 104.0f, 106.0f), p + v));
        Assert.True(Point._ArePointsClose(new Point(102.0f, 104.0f, 106.0f), v + p));
    }

    [Fact]
    public void TestPointDiff()
    {
        var p1 = new Point(3.0f, 2.0f, 1.0f);
        var p2 = new Point(101.0f, 102.0f, 103.0f);
        
        Assert.Equal(new Vector(98.0f, 100.0f, 102.0f), p2 - p1);
        
        //Implementare Test con funzione _AreCloseVector
    }

    [Fact]
    public void TestPointVectorDiff()
    {
        var v = new Vector(3.0f, 2.0f, 1.0f);
        var p = new Point(101.0f, 102.0f, 103.0f);
        
        Assert.Equal(new Point(98.0f, 100.0f, 102.0f), p - v);
        
        Assert.True(Point._ArePointsClose(new Point(98.0f, 100.0f, 102.0f), p - v));
    }

    [Fact]
    public void PointToVectorTest()
    {
        var p = new Point(1.0f, 2.0f, 3.0f);
        
        Assert.Equal(new Vector(1.0f, 2.0f, 3.0f), p.ToVector());
    }
}