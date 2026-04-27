namespace TracerLib;

public class World : Shape
{
    public List<Shape> Shapes { get; private set; }

    public World(List<Shape> shapes)
    {
        Shapes = shapes;
    }

    public void Add(Shape shape)
    {
        Shapes.Add(shape);
    }

    public override HitRecord? RayIntersection(Ray ray)
    {
        HitRecord? closest = null;
        foreach (var shape in Shapes)
        {
            var intersection = shape.RayIntersection(ray);
            if (intersection == null)
            {
                continue;
            }

            if (closest == null || intersection.Value.T < closest.Value.T)
            {
                closest = intersection;
            }
        }
        return closest;
    }
}