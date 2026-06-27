// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// A base class that implements the rendering algorithm.
/// </summary>
public abstract class Renderer
{
    /// <summary>
    /// Evaluates the surface color at the ray intersection point
    /// using an algorithm that depends on the concrete <see cref="Renderer"/> implementation.
    /// </summary>
    /// <param name="ray">The <see cref="Ray"/> that intersects the scene.</param>
    /// <returns>The computed <see cref="Color"/> at the intersection point.</returns>
    public abstract Color RenderFunction(Ray ray);
}

/// <summary>
/// A renderer that renders all objects using a single white and a background color (on/off rendering).
/// </summary>
public class OnOffRenderer : Renderer
{
    /// <summary>
    /// The world containing all the shapes of the scene.
    /// </summary>
    public World World { get; set; }

    /// <summary>
    /// The color returned when a ray does not intersect any <see cref="Shape"/> in the scene.
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Constructs a renderer that draws all objects in white.
    /// </summary>
    /// <param name="world">The world containing the objects to render.</param>
    /// <param name="backgroundColor">The background color. If not provided, defaults to black.</param>
    public OnOffRenderer(World world, Color? backgroundColor = null)
    {
        World = world;
        BackgroundColor = backgroundColor ?? new Color(0.0f, 0.0f, 0.0f);
    }

    /// <summary>
    /// Returns a white <see cref="Color"/> if the ray intersects a <see cref="Shape"/> of the scene
    /// otherwise returns the <see cref="BackgroundColor"/>.
    /// </summary>
    /// <param name="ray">The <see cref="Ray"/> that intersects the scene.</param>
    /// <returns></returns>
    public override Color RenderFunction(Ray ray)
    {
        //find the first intersection with the closest shape relative to the origin of the ray.
        return World.FindIntersection(ray) != null ? new Color(1.0f, 1.0f, 1.0f) : BackgroundColor;
    }
}

/// <summary>
/// A renderer that colors the objects based only on their <see cref="Pigment"/>, without reflecting any rays.
/// </summary>
public class FlatRenderer : Renderer
{
    /// <summary>
    /// The world containing all the shapes of the scene.
    /// </summary>
    public World World { get; set; }

    /// <summary>
    /// The color returned when a ray does not intersect any <see cref="Shape"/> in the scene.
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Constructs a renderer that colors the objects based only on their <see cref="Pigment"/>.
    /// </summary>
    /// <param name="world">The world containing the objects to render.</param>
    /// <param name="backgroundColor">The background color. If not provided, defaults to black.</param>
    public FlatRenderer(World world, Color? backgroundColor = null)
    {
        World = world;
        BackgroundColor = backgroundColor ?? new Color(0.0f, 0.0f, 0.0f);
    }

    /// <summary>
    /// Evaluates the surface color at the ray intersection point using only the shape's <see cref="Pigment"/>.
    /// </summary>
    /// <param name="ray">The <see cref="Ray"/> that intersects the scene.</param>
    /// <returns>The evaluated <see cref="Color"/> at the intersection point.</returns>
    public override Color RenderFunction(Ray ray)
    {
        //find the first intersection with the closest shape relative to the origin of the ray.
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
/// A renderer that uses a path tracing algorithm, tracing rays recursively through
/// the scene to estimate radiance at the observer via Monte Carlo integration.
/// </summary>
public class PathTracingRenderer : Renderer
{
    /// <summary>
    /// The world containing all the shapes of the scene.
    /// </summary>
    public World World { get; set; }

    /// <summary>
    /// A random generator used for the Monte Carlo integration.
    /// </summary>
    public PCG Pcg { get; set; }

    /// <summary>
    /// The color returned when a ray does not intersect any <see cref="Shape"/> in the scene.
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// The number of scattered rays for each intersection.
    /// </summary>
    public int NumRay { get; set; }

    /// <summary>
    /// Maximum number of ray reflections before terminating the path and returning the <see cref="BackgroundColor"/>.
    /// </summary>
    public int MaxDepth { get; set; }

    /// <summary>
    /// Number of ray reflections after which the Russian roulette algorithm is applied.
    /// </summary>
    public int RussianRouletteStartDepth { get; set; }

    /// <summary>
    /// Optional fixed probability for the Russian roulette algorithm.
    /// When null, the probability is computed dynamically at each recursive call of <see cref="RenderFunction"/>.
    /// </summary>
    public float? RussianRouletteFixedProbability { get; set; }

    /// <summary>
    /// Constructs a renderer that uses a path tracing algorithm.
    /// See the class <see cref="PathTracingRenderer"/> for more information.
    /// </summary>
    public PathTracingRenderer(World world, PCG? pcg = null, Color? backgroundColor = null, int numRay = 10,
        int maxDepth = 2, int russianRouletteStartDepth = 3, float? russianRouletteProbability = null)
    {
        World = world;
        Pcg = pcg ?? new PCG();
        BackgroundColor = backgroundColor ?? new Color(0.0f, 0.0f, 0.0f);
        NumRay = numRay;
        RussianRouletteStartDepth = russianRouletteStartDepth;
        MaxDepth = maxDepth;
        RussianRouletteFixedProbability = russianRouletteProbability;
    }
    
    /// <summary>
    /// Evaluates the surface color at the ray intersection point using Monte Carlo integration
    /// and the Russian roulette algorithm.
    /// </summary>
    /// <param name="ray">The <see cref="Ray"/> that intersects the scene.</param>
    /// <returns>The computed <see cref="Color"/> at the intersection point.</returns>
    public override Color RenderFunction(Ray ray)
    {
        if (ray.Depth > MaxDepth) return BackgroundColor;

        // find the first intersection with the closest shape relative to the origin of the ray.
        HitRecord? hit = World.FindIntersection(ray);

        if (hit == null) return BackgroundColor;

        Material material = hit.Value.Shape.Material;
        Color hitColor = material.Pigment.GetColor(hit.Value.SurfacePosition);
        Color emittedRadiance = material.EmittedRadiance.GetColor(hit.Value.SurfacePosition);

        //maybe it's more efficient to put it inside the 'if' of the russian roulette algorithm
        //and substitute in
        //if (maxLum > 0.0f)
        //the equivalent condition
        //if (hitColor.R > 0 && hitColor.G > 0 && hitColor.B > 0)
        float maxLum = MathF.Max(MathF.Max(hitColor.R, hitColor.G), hitColor.B);

        // q is the probability used for Russian roulette
        // if the user has set RussianRouletteFixedProbability then it's used that,
        // otherwise is computed each time:
        float q = RussianRouletteFixedProbability ?? MathF.Max(0.05f, 1 - maxLum);

        // Russian roulette algorithm
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

        Color newRadiance;
        Color cumRadiance = new Color();
        Ray newRay;

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

        // emittedRadiance + reflectedRadiance
        return emittedRadiance + cumRadiance * (1.0f / NumRay);
    }
}