// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

public abstract class Shape
{
    public abstract HitRecord? RayIntersection(Ray ray);
}

public class Sphere : Shape
{
    public override HitRecord? RayIntersection(Ray ray)
    {
        
    }
}