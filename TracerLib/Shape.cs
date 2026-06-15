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
    /// depending on the direction dir of the Ray incident on the <c>Point</c> p of the unit sphere.
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

    public Normal _PlaneNormal(Vector dir)
    {
        var normal = new Normal(0, 0, 1);
        return dir.Z < 0 ? normal : -normal;
    }

    public Vector2D _PlanePointToUV(Point p)
    {
        return new Vector2D(p.X - MathF.Floor(p.X), p.Y - MathF.Floor(p.Y));
    }

    public override HitRecord? RayIntersection(Ray ray)
    {
        var invRay = Transform.Inverse() * ray;
        var origin = invRay.Origin;
        var dir = invRay.Dir;
        var t = -origin.Z / dir.Z;

        if (t > invRay.Tmin && t < invRay.Tmax)
        {
            var intersectionPoint = invRay.At(t);
            return new HitRecord(
                Transform * intersectionPoint,
                Transform * _PlaneNormal(dir),
                _PlanePointToUV(intersectionPoint),
                ray,
                t
                );
        }
        return null; //no intersection
    }
}