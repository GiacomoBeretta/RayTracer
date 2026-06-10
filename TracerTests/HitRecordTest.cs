using TracerLib;

namespace TracerTests;

public class HitRecordTest
{
    [Fact]
    public void TestConstructor()
    {
        Point point = new Point(8, 1, -9.125f);
        Normal normal = new Normal(1, 0, 0);
        Vector2D surfacePoint = new Vector2D(4, 1);
        Ray ray = new Ray(new Point(), new Vector(5, 2, 6));
        Sphere sphere = new Sphere();
        HitRecord record = new HitRecord(point, sphere, normal, surfacePoint, ray, 2);
        Assert.Equal(point, record.WorldPoint);
        Assert.Equal(sphere, record.Shape);
        Assert.Equal(normal, record.SurfaceNormal);
        Assert.Equal(surfacePoint, record.SurfacePosition);
        Assert.Equal(ray, record.IncomingRay);
        Assert.Equal(2, record.T);
    }

    [Fact]
    public void TestAreHitRecordsClose()
    {
        Point point1 = new Point(8, 1, -9.125f);
        Normal normal1 = new Normal(1, 0, 0);
        Vector2D surfacePoint1 = new Vector2D(4, 1);
        Ray ray1 = new Ray(new Point(), new Vector(5, 2, 6));
        Sphere sphere = new Sphere();
        HitRecord record1 = new HitRecord(point1, sphere, normal1, surfacePoint1, ray1, 2);

        Point point2 = new Point(8, 1, -9.12507f);
        Normal normal2 = new Normal(1, 0.000003f, 0);
        Vector2D surfacePoint2 = new Vector2D(4.00003f, 1);
        Ray ray2 = new Ray(new Point(), new Vector(5, 2, 6.00002f));
        Sphere sphere2 = new Sphere();
        HitRecord record2 = new HitRecord(point2, sphere, normal2, surfacePoint2, ray2, 2.00004f);

        Assert.True(HitRecord._AreHitRecordsClose(record1, record2, 1e-3f));
        Assert.False(HitRecord._AreHitRecordsClose(record2, record1, 1e-5f));
    }
}