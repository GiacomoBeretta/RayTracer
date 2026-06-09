// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// Base class for all the 3D geometric shapes that will compose the scene.
/// </summary>
public abstract class Shape
{
    public Material Material { get; }

    public Shape()
    {
        Material = new Material();
    }

    public Shape(Material material)
    {
        Material = material;
    }

    /// <summary>
    /// Finds the closest intersection between the specified ray and *this* shape, otherwise returns a null value.
    /// </summary>
    /// <param name="ray">The <see cref="Ray"/> to test for intersections.</param>
    /// <returns>A <see cref="HitRecord"/> describing the closest intersection, or null if no intersection exists.</returns>
    public abstract HitRecord? FindIntersection(Ray ray);
    
    public abstract bool _IsCloseTo(Shape s, float epsilon = 1e-5f);

    /// <summary>
    /// Constructs a local orthonormal basis from the specified normal vector.
    /// The resulting basis satisfies e3 = normal.
    /// </summary>
    /// <param name="normal">
    /// The <see cref="Normal"/> vector used to construct the orthonormal basis.
    /// </param>
    /// <param name="e1">
    /// The first unit vector of the basis.
    /// </param>
    /// <param name="e2">
    /// The second unit vector of the basis.
    /// </param>
    /// <param name="e3">
    /// The third unit vector of the basis, equal to <paramref name="normal"/>.
    /// </param>
    public static void CreateONB(Normal normal, out Vector e1, out Vector e2, out Vector e3)
    {
        int sign = normal.Z > 0.0f ? 1 : -1;
        float a = -1.0f / (sign + normal.Z);
        float b = normal.X * normal.Y * a;

        e1 = new Vector(1.0f + sign * normal.X * normal.X * a, sign * b, -sign * normal.X);
        e2 = new Vector(b, sign + normal.Y * normal.Y * a, -normal.Y);
        e3 = new Vector(normal.X, normal.Y, normal.Z);
    }
}

/// <summary>
/// A 3D unit Sphere.
/// The transform field allows to represent also translated and rotated ellipsoids.
/// </summary>
public class Sphere : Shape
{
    public Transformation Transform { get; }
    
    public Sphere()
    {
        Transform = new Transformation();
    }

    public Sphere(Transformation transform)
    {
        this.Transform = transform;
    }
    
    public Sphere(Material material) : base(material)
    {
        Transform = new Transformation();
    }
    
    public Sphere(Transformation transform, Material material) : base(material)
    {
        Transform = transform;
    }

    /// <summary>
    /// Returns true if the shape passed as parameter is a Sphere and if it has the Transform member close within epsilon
    /// </summary>
    /// <param name="s"></param>
    /// <param name="epsilon"></param>
    /// <returns></returns>
    public override bool _IsCloseTo(Shape s, float epsilon = 1E-05F)
    {
        if (s.GetType() != typeof(Sphere))
        {
            throw new ArgumentException("The shape must be of type Sphere");
        }

        Sphere sphere = (Sphere)s;
        if (!Transformation.AreTransformationsClose(this.Transform, sphere.Transform, epsilon))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns the normal to the Sphere surface
    /// depending on the direction dir of the Ray incident on the Point p of the unit sphere.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public Normal _SphereNormal(Point p, Vector dir)
    {
        Normal normal = new Normal(p.X, p.Y, p.Z);
        return normal * dir < 0 ? normal : -normal;
    }

    /// <summary>
    /// Returns the normalized longitude and colatitude of the <c>Point</c> p on the unit sphere.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public Vector2D _SpherePointToUV(Point p)
    {
        return new Vector2D(MathF.Atan2(p.Y, p.X) / (2 * MathF.PI), MathF.Acos(p.Z) / MathF.PI);
    }

    /// <summary>
    /// Returns a <c>HitRecord</c> object if there is an intersection between the <c>Ray</c> passed as argument
    /// and *this* shape, otherwise returns a null value.
    /// See <c>HitRecord</c> for more information.
    /// (For the sphere the (U,V) coordinates are longitude and colatitude, normalized).
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public override HitRecord? FindIntersection(Ray ray)
    {
        // instead of transforming the sphere to represent all sorts of ellipsoids
        // we transform the ray with the inverse transformation
        Ray invRay = (Transform.Inverse()) * ray;
        Vector dir = invRay.Dir;
        Vector origin = invRay.Origin.ToVector();
        float delta_4 = (origin * dir) * (origin * dir) - dir.SquaredNorm() * (origin.SquaredNorm() - 1);
        // if delta_4 < 0 there are no intersection, if delta_4 == 0 there is no reflection
        // then we take only one of the two solutions depending on whether they represent
        // intersections on the line that are behind the origin of the ray.
        // See Ray struct for more information on tmin and tmax
        if (delta_4 > 0)
        {
            float sqrt_delta_4 = MathF.Sqrt(delta_4);
            float t1 = -(origin * dir + sqrt_delta_4) / dir.SquaredNorm();
            if (t1 > invRay.Tmin && t1 < invRay.Tmax)
            {
                Point intersectionPoint = invRay.At(t1); // intersection on the unit sphere
                return new HitRecord
                (
                    Transform * intersectionPoint,
                    this,
                    Transform * _SphereNormal(intersectionPoint, dir),
                    _SpherePointToUV(intersectionPoint),
                    ray,
                    t1
                );
            }

            float t2 = (-(origin * dir) + sqrt_delta_4) / dir.SquaredNorm();
            if (t2 > invRay.Tmin && t2 < invRay.Tmax)
            {
                Point intersectionPoint = invRay.At(t2); // intersection on the unit sphere
                return new HitRecord
                (
                    Transform * intersectionPoint,
                    this,
                    Transform * _SphereNormal(intersectionPoint, dir),
                    _SpherePointToUV(intersectionPoint),
                    ray,
                    t2
                );
            }
        }

        return null; //no intersection
    }
}

public class Plane : Shape
{
    public Transformation Transform { get; }

    public Plane()
    {
        Transform = new Transformation();
    }

    public Plane(Transformation transform)
    {
        Transform = transform;
    }

    public Plane(Transformation transform, Material material) : base(material)
    {
        Transform = transform;
    }

    /// <summary>
    /// Returns true if the shape passed as argument is a Plane and if it has the Transform member close within epsilon
    /// </summary>
    /// <param name="s"></param>
    /// <param name="epsilon"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public override bool _IsCloseTo(Shape s, float epsilon = 1E-05F)
    {
        if (s.GetType() != typeof(Plane))
        {
            throw new ArgumentException("The shapes must be of type Plane");
        }

        Plane plane = (Plane)s;
        if (!Transformation.AreTransformationsClose(this.Transform, plane.Transform, epsilon))
        {
            return false;
        }

        return true;
    }

    public Normal _PlaneNormal(Vector dir)
    {
        var normal = new Normal(0, 0, 1);
        return dir.Z < 0 ? normal : -normal;
    }

    public Vector2D _PlanePointToUV(Point p)
    {
        return new Vector2D(p.X - MathF.Floor(p.X), p.Y - MathF.Floor(p.Y));
    }

    public override HitRecord? FindIntersection(Ray ray)
    {
        Ray invRay = Transform.Inverse() * ray;
        Point origin = invRay.Origin;
        Vector dir = invRay.Dir;
        float t = -origin.Z / dir.Z;

        if (t > invRay.Tmin && t < invRay.Tmax)
        {
            Point intersectionPoint = invRay.At(t);
            return new HitRecord(
                Transform * intersectionPoint,
                this,
                Transform * _PlaneNormal(dir),
                _PlanePointToUV(intersectionPoint),
                ray,
                t
            );
        }

        return null; //no intersection
    }
}