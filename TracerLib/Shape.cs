// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// An abstract class to represent all the possible 3D geometric shapes that will compose the scene
/// </summary>
public abstract class Shape
{
    /// <summary>
    /// Returns a <c>HitRecord</c> object if there is an intersection between the <c>Ray</c> passed as argument
    /// and *this* shape, otherwise returns a null value.
    /// See <c>HitRecord</c> for more information.
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public abstract HitRecord? RayIntersection(Ray ray);
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

    /// <summary>
    /// Returns the normal to the Sphere surface
    /// depending on the direction dir of the Ray incident on the <c>Point</c> p.
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
    public override HitRecord? RayIntersection(Ray ray)
    {
        // instead of transforming the sphere to represent all sorts of ellipsoids
        // we transform the ray with the inverse transformation
        Ray T_ray = (Transform.Inverse()) * ray;
        Vector dir = T_ray.Dir;
        Vector origin = T_ray.Origin.ToVector();
        float delta_4 = (origin * dir) * (origin * dir) - dir.SquaredNorm() * (origin.SquaredNorm() - 1);
        // if delta_4 < 0 there are no intersection, if delta_4 == 0 there is no reflection
        // then we take only one of the two solutions depending on whether they represent
        // intersections on the line that are behind the origin of the ray.
        // See Ray struct for more information on tmin and tmax
        if (delta_4 > 0)
        {
            float sqrt_delta_4 = MathF.Sqrt(delta_4);
            float t1 = -(origin * dir + sqrt_delta_4) / dir.SquaredNorm();
            if (t1 > T_ray.Tmin && t1 < T_ray.Tmax)
            {
                Point p = ray.At(t1);
                return new HitRecord(p,
                    Transform * _SphereNormal(p, dir),
                    _SpherePointToUV(p),
                    ray,
                    t1);
            }

            float t2 = (-(origin * dir) + sqrt_delta_4) / dir.SquaredNorm();
            if (t2 > T_ray.Tmin && t2 < T_ray.Tmax)
            {
                Point p = ray.At(t2);
                return new HitRecord(p,
                    Transform * _SphereNormal(p, dir),
                    _SpherePointToUV(p),
                    ray,
                    t2);
            }
        }

        return null; //no intersection
    }
}