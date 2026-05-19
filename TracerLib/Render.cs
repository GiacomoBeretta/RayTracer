namespace TracerLib;

public static class Render
{
    public static Color OnOff(World world, Ray ray)
    {
        return world.RayIntersection(ray) != null ? new Color(1.0f, 1.0f, 1.0f) : new Color(0.0f, 0.0f, 0.0f);
    }

    public static Color Flat(World world, Ray ray)
    {
        var hit = world.RayIntersection(ray);

        if (hit == null)
        {
            return new Color(0.0f, 0.0f, 0.0f);
        }

        var material = hit.Value.Shape.Material;

        return material.Pigment.GetColor(hit.Value.SurfacePoint) +
               material.EmittedRadiance.GetColor(hit.Value.SurfacePoint);
    }
}