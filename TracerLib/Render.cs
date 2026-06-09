// This file is release under EUPL_v1.2 license. See LICENSE.md

//o forse è meglio chiamarli renderer al posto che render?
namespace TracerLib;

public abstract class Render
{
    public abstract Color RenderFunction(Ray ray);
}

public class OnOffRender : Render
{
    public World World { get; set; }
    public Color BackgroundColor { get; set; }
    
    public OnOffRender(World world, Color? backgroundColor = null)
    {
        World = world;
        BackgroundColor = backgroundColor ?? new Color(0.0f, 0.0f, 0.0f);
    }

    public override Color RenderFunction(Ray ray)
    {
        return World.FindIntersection(ray) != null ? new Color(1.0f, 1.0f, 1.0f) : BackgroundColor;
    }
}

public class FlatRender : Render
{
    public World World { get; set; }
    public Color BackgroundColor { get; set; }

    public FlatRender(World world, Color? backgroundColor = null)
    {
        World = world;
        BackgroundColor = backgroundColor ?? new Color(0.0f, 0.0f, 0.0f);
    }

    public override Color RenderFunction(Ray ray)
    {
        HitRecord? hit = World.FindIntersection(ray);

        if (hit == null)
        {
            return BackgroundColor;
        }

        Material material = hit.Value.Shape.Material;

        return material.Pigment.GetColor(hit.Value.SurfacePoint) +
               material.EmittedRadiance.GetColor(hit.Value.SurfacePoint);
    }
}

public class PathTracer : Render
{
    public World World { get; set; }
    public PCG Pcg { get; set; }
    public Color BackgroundColor { get; set; } 
    public int NumRay { get; set; }
    public int RussianRouletteStop { get; set; }
    public int MaxDepth { get; set; }

    public PathTracer(World world, PCG? pcg = null, Color? backgroundColor = null, int numRay = 10,
        int russianRouletteStop = 3, int maxDepth = 2)
    {
        World = world;
        Pcg = pcg ?? new PCG();
        BackgroundColor = backgroundColor ?? new Color(0.0f, 0.0f, 0.0f);
        NumRay = numRay;
        RussianRouletteStop = russianRouletteStop;
        MaxDepth = maxDepth;
    }

    public override Color RenderFunction(Ray ray)
    {
        if (ray.Depth > MaxDepth) return BackgroundColor;

        HitRecord? hit = World.FindIntersection(ray);

        if (hit == null) return BackgroundColor;

        Material material = hit.Value.Shape.Material;
        Color color = material.Pigment.GetColor(hit.Value.SurfacePoint);
        Color radiance = material.EmittedRadiance.GetColor(hit.Value.SurfacePoint);

        float lum = MathF.Max(MathF.Max(color.R, color.G), color.B);
        
        //Russian roulette
        if (ray.Depth > RussianRouletteStop)
        {
            if (Pcg.RandomFloat() > lum)
            {
                color *= 1.0f / (1.0f - lum);
            }
            else
            {
                return radiance;
            }
        }

        Color cumRadiance = new Color();

        Ray newRay = new Ray();
        if (lum > 0.0f)
        {
            for(int i = 0; i < NumRay; i++)
            {
                newRay = material.Brdf.ScatterRay(Pcg, hit.Value.IncomingRay.Dir, hit.Value.WorldPoint, hit.Value.SurfaceNormal, ray.Depth +1);
            }

            Color newRadiance = RenderFunction(newRay);
            cumRadiance += color * newRadiance;
        }

        return radiance + cumRadiance * (1.0f / NumRay);
    }
}


