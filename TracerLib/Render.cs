// This file is release under EUPL_v1.2 license. See LICENSE.md

//o forse è meglio chiamarli renderer al posto che render?

namespace TracerLib;

/// <summary>
/// A base class that implements the render algorithm.
/// </summary>
public abstract class Renderer
{
    public abstract Color RenderFunction(Ray ray);
}

/// <summary>
/// A renderer that renders all objects using a single white color (on/off rendering).
/// </summary>
public class OnOffRenderer : Renderer
{
    public World World { get; set; }
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Initializes a renderer that draws all objects in white.
    /// </summary>
    /// <param name="world">The world containing the objects to render.</param>
    /// <param name="backgroundColor">
    /// The background color. If null, defaults to black.
    /// </param>
    public OnOffRenderer(World world, Color? backgroundColor = null)
    {
        World = world;
        BackgroundColor = backgroundColor ?? new Color(0.0f, 0.0f, 0.0f);
    }

    /// <summary>
    /// Returns a white color if the ray intersects a <see cref="Shape"/> otherwhise returns the <see cref="BackgroundColor"/>.
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public override Color RenderFunction(Ray ray)
    {
        return World.FindIntersection(ray) != null ? new Color(1.0f, 1.0f, 1.0f) : BackgroundColor;
    }
}

public class FlatRenderer : Renderer
{
    public World World { get; set; }
    public Color BackgroundColor { get; set; }

    public FlatRenderer(World world, Color? backgroundColor = null)
    {
        World = world;
        BackgroundColor = backgroundColor ?? new Color(0.0f, 0.0f, 0.0f);
    }

    //da completare
    /// <summary>
    /// Returns the color of the shape intersected by the ray, evaluating 
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public override Color RenderFunction(Ray ray)
    {
        HitRecord? hit = World.FindIntersection(ray);

        if (hit == null)
        {
            return BackgroundColor;
        }

        Material material = hit.Value.Shape.Material;

        return material.Pigment.GetColor(hit.Value.SurfacePosition) +
               material.EmittedRadiance.GetColor(hit.Value.SurfacePosition);
    }
}

/// <summary>
/// A renderer that uses a path tracing algorithm, i.e. follows each ray in his reflections to evaluate the radiance at the observer. 
/// </summary>
public class PathTracingRenderer : Renderer
{
    /// <summary>
    /// Contains all the shapes of the scene.
    /// </summary>
    public World World { get; set; }

    /// <summary>
    /// A random generator.
    /// </summary>
    public PCG Pcg { get; set; }

    public Color BackgroundColor { get; set; }
    public int NumRay { get; set; }

    /// <summary>
    /// Number of ray reflections after which the Russian roulette algorithm is applied.
    /// </summary>
    public int RussianRouletteStartDepth { get; set; }

    /// <summary>
    /// Maximum number of ray reflections before terminating the path and returning the background color.
    /// </summary>
    public int MaxDepth { get; set; }

    /// <summary>
    /// Optional fixed probability for the Russian roulette algorithm.
    /// When null, the probability is computed dynamically at each recursive call of <see cref="RenderFunction"/>.
    /// </summary>
    public float? RussianRouletteFixedProbability { get; set; }

    public PathTracingRenderer(World world, PCG? pcg = null, Color? backgroundColor = null, int numRay = 10,
        int russianRouletteStop = 3, int maxDepth = 2, float? russianRouletteProb = null)
    {
        World = world;
        Pcg = pcg ?? new PCG();
        BackgroundColor = backgroundColor ?? new Color(0.0f, 0.0f, 0.0f);
        NumRay = numRay;
        RussianRouletteStartDepth = russianRouletteStop;
        MaxDepth = maxDepth;
        RussianRouletteFixedProbability = russianRouletteProb;
    }

    public override Color RenderFunction(Ray ray)
    {
        if (ray.Depth > MaxDepth) return BackgroundColor;

        HitRecord? hit = World.FindIntersection(ray);

        if (hit == null) return BackgroundColor;

        Material material = hit.Value.Shape.Material;
        Color hitColor = material.Pigment.GetColor(hit.Value.SurfacePosition);
        Color emittedRadiance = material.EmittedRadiance.GetColor(hit.Value.SurfacePosition);

        //maybe it's more efficient to put it inside the if of the russian roulette algorithm
        //and substitute in
        //if (maxLum > 0.0f)
        //the equivalent condition
        //if (hitColor.R > 0 && hitColor.G > 0 && hitColor.B > 0)
        float maxLum = MathF.Max(MathF.Max(hitColor.R, hitColor.G), hitColor.B);

        float q = RussianRouletteFixedProbability ?? MathF.Max(0.05f, 1 - maxLum);

        //Russian roulette algorithm
        if (ray.Depth > RussianRouletteStartDepth)
        {
            if (Pcg.RandomFloat() < q)
            {
                // truncate the path
                return emittedRadiance;
            }
            else
            {
                // keep the recursion going and boost the value of reflected radiance
                // to compensate for other potentially discarded rays.
                hitColor *= 1.0f / (1.0f - q);
            }
        }

        Color newRadiance = new Color();
        Color cumRadiance = new Color();
        Ray newRay = new Ray();

        // if the RGB values of hitColor are 0 then the contribution to cumRadiance is null.
        if (maxLum > 0.0f)
        {
            for (int i = 0; i < NumRay; i++)
            {
                newRay = material.Brdf.ScatterRay(Pcg, hit.Value.IncomingRay.Dir, hit.Value.WorldPoint,
                    hit.Value.SurfaceNormal, ray.Depth + 1);

                newRadiance = RenderFunction(newRay);
                // BRDF * incident radiance (hadamard product) they are 3 equations for the 3 colors.
                cumRadiance += hitColor * newRadiance;
            }
        }

        return emittedRadiance + cumRadiance * (1.0f / NumRay);
    }
}