// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// Base class for all the 3D geometric shapes that will compose the scene.
/// </summary>
public abstract class Shape
{
    /// <summary>
    /// Specifies the optical properties of a surface material,
    /// including its reflection model and any emitted radiance.
    /// </summary>
    public Material Material { get; }

    /// <summary>
    /// Base abstract class constructor that initialize a shape with a uniform black color.
    /// </summary>
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

    /// <summary>
    /// Returns whether this shape is approximately equal to the specified shape within a given tolerance.
    /// The <see cref="Material"/> is not considered for the comparison.
    /// </summary>
    /// <param name="s">The shape to compare to this.</param>
    /// <param name="epsilon">Tolerance threshold used for floating-point comparison.</param>
    /// <returns></returns>
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
/// A 3D unit Sphere centered at the origin.
/// The <see cref="Transform"/> property allows to represent also translated and rotated ellipsoids.
/// </summary>
public class Sphere : Shape
{
    /// <summary>
    /// The transformation applied to the unit sphere.
    /// </summary>
    public Transformation Transform { get; }

    /// <summary>
    /// Constructs a unit sphere centered at the origin.
    /// The material is initialized by the base class and defaults to a uniform black color.
    /// </summary>
    public Sphere() : base()
    {
        Transform = new Transformation();
    }

    /// <summary>
    /// Initializes a new instance of the Sphere class with the specified transform.
    /// The material is initialized by the base class and defaults to a uniform black color.
    /// </summary>
    /// <param name="transform">The transformation to apply to this sphere.</param>
    public Sphere(Transformation transform) : base()
    {
        Transform = transform;
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
    /// Determines whether this sphere is geometrically close to another sphere,
    /// comparing only their Transform within a given tolerance.
    /// Material properties are ignored.
    /// </summary>
    /// <param name="s">The sphere to compare against.</param>
    /// <param name="epsilon">Tolerance used for floating-point comparisons.</param>
    /// <returns>
    /// True if the transforms of the two spheres are approximately equal within epsilon; otherwise false.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided shape is not of type Sphere.
    /// </exception>
    public override bool _IsCloseTo(Shape s, float epsilon = 1E-05F)
    {
        if (s.GetType() != typeof(Sphere))
        {
            throw new ArgumentException("The shape must be of type Sphere");
        }

        Sphere sphere = (Sphere)s;
        if (!Transformation.AreTransformationsClose(Transform, sphere.Transform, epsilon))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns the surface normal of the unit sphere at point p,
    /// oriented opposite to the incident direction.
    /// </summary>
    /// <param name="p">The intersection point between the sphere and the ray.</param>
    /// <param name="incidentDir">Incident direction (typically the ray direction at intersection).</param>
    /// <returns>The correctly oriented normal at the given point.</returns>
    public Normal _SphereNormal(Point p, Vector incidentDir)
    {
        Normal normal = new Normal(p.X, p.Y, p.Z);
        return normal * incidentDir < 0 ? normal : -normal;
    }

    /// <summary>
    /// Returns the normalized longitude and colatitude of the Point p on the unit sphere.
    /// The (u,v) coordinates of the 2D vector are in [0,1]x[0,1].
    /// </summary>
    /// <param name="p">A point on the unit sphere.</param>
    /// <returns>A <see cref="Vector2D"/> whose U component is the normalized longitude
    /// in the range [0,1] and whose V component is the normalized colatitude
    /// in the range [0,1], with V = 0 at the north pole and V = 1 at the south pole.
    /// </returns>
    public Vector2D _SpherePointToUV(Point p)
    {
        // Atan2 codomain is (-pi, pi] and for Acos is [0,pi].
        // so that u is in (-0.5, 0.5], but we want u, v in [0,1]
        // Then we put in correspondence the angles in [-pi, 0] with the angles in [pi, 2pi]
        // Then if the angle is negative we translate by 2pi. We did this with u, translating it by 1 if negative.
        float u = MathF.Atan2(p.Y, p.X) / (2 * MathF.PI);
        float v = MathF.Acos(p.Z) / MathF.PI;

        if (u < 0)
        {
            u += 1;
        }

        return new Vector2D(u, v);
    }

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

/// <summary>
/// The XY plane.
/// The <see cref="Transform"/> property can be used to represent any plane in 3D space.
/// </summary>
public class Plane : Shape
{
    /// <summary>
    /// The transformation applied to the unit sphere.
    /// </summary>
    public Transformation Transform { get; }
    
    /// <summary>
    /// Constructs the XY plane.
    /// The material is initialized by the base class and defaults to a uniform black color.
    /// </summary>
    public Plane() : base()
    {
        Transform = new Transformation();
    }
    
    /// <summary>
    /// Initializes a new instance of the Plane class with the specified transform.
    /// The material is initialized by the base class and defaults to a uniform black color.
    /// </summary>
    /// <param name="transform">The transformation to apply to this plane.</param>
    public Plane(Transformation transform) : base()
    {
        Transform = transform;
    }

    public Plane(Transformation transform, Material material) : base(material)
    {
        Transform = transform;
    }

    /// <summary>
    /// Determines whether this plane is geometrically close to another plane,
    /// comparing only their Transform within a given tolerance.
    /// Material properties are ignored.
    /// </summary>
    /// <param name="s">The plane to compare against.</param>
    /// <param name="epsilon">Tolerance used for floating-point comparisons.</param>
    /// <returns>
    /// True if the transforms of the two planes are approximately equal within epsilon; otherwise false.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided shape is not of type Plane.
    /// </exception>
    public override bool _IsCloseTo(Shape s, float epsilon = 1E-05F)
    {
        if (s.GetType() != typeof(Plane))
        {
            throw new ArgumentException("The shapes must be of type Plane");
        }

        Plane plane = (Plane)s;
        if (!Transformation.AreTransformationsClose(Transform, plane.Transform, epsilon))
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