// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// Represents a scene containing all the shapes to be rendered.
/// </summary>
public class World
{
    /// <summary>
    /// The collection of shapes contained in the world.
    /// </summary>
    public List<Shape> Shapes { get; private set; }
    
    /// <summary>
    /// Initializes a new world containing the specified shapes.
    /// </summary>
    /// <param name="shapes">
    /// The shapes to add to the world. If <c>null</c>, an empty List of shapes is created.
    /// </param>
    public World(List<Shape>? shapes = null)
    {
        Shapes = shapes ?? new List<Shape>();
    }
    
    /// <summary>
    /// Add the shape specified to the list of shapes.
    /// </summary>
    /// <param name="shape"></param>
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
        foreach (Shape shape in Shapes)
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