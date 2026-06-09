namespace TracerLib;

/// <summary>
/// A class that contains all the shapes of the scene to render.
/// </summary>
public class World
{
    public List<Shape> Shapes { get; private set; }
    
    public World(List<Shape>? shapes = null)
    {
        Shapes = shapes ?? new List<Shape>();
    }
    
    public void Add(Shape shape)
    {
        Shapes.Add(shape);
    }
    
    /// <summary>
    /// Finds the closest intersection between the specified ray and the shapes in the scene.
    /// Returns null if the ray does not intersect any shape.
    /// </summary>
    /// <param name="ray">The <see cref="Ray"/> to test for intersections.</param>
    /// <returns>A <see cref="HitRecord"/> describing the closest intersection, or null if no intersection exists.</returns>
    public HitRecord? FindIntersection(Ray ray)
    {
        HitRecord? closest = null;
        foreach (var shape in Shapes)
        {
            HitRecord? intersection = shape.FindIntersection(ray);
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