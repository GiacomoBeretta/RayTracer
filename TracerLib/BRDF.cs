// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// A class representing the Bidirectional Reflectance Distribution Function.
/// That is the ratio between the radiance leaving a surface and the irradiance received. So it's a dimensionless number.
/// Describes how light is reflected by the material.
/// </summary>
public abstract class BRDF
{
    /// <summary>
    /// The Pigment property represents the texture of the surface
    /// </summary>
    public Pigment Pigment;

    // forse questo si può togliere?
    //public abstract Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv);
    
    /// <summary>
    /// Base abstract class constructor that initialize a BRDF with a uniform black color.
    /// </summary>
    protected BRDF()
    {
        Pigment = new UniformPigment();
    }

    protected BRDF(Pigment pigment)
    {
        Pigment = pigment;
    }

    // public abstract Ray ScatterRay(PCG pcg, Ray incidentRay, Point interactionPoint, Normal normal);
    /// <summary>
    /// Returns a <c>Ray</c> generated according to the type of <see cref="BRDF"/>.
    /// The new ray will have the provided <c>depth</c>.
    /// </summary>
    /// <param name="pcg"> Random number generator used for Monte Carlo sampling.</param>
    /// <param name="incidentVector">Direction of the incoming <c>Ray</c>.</param>
    /// <param name="interactionPoint"><c>Point</c> on the surface where the scattering event occurs.</param>
    /// <param name="normal"> Surface <c>Normal</c> used to orient the new ray.</param>
    /// <param name="depth"> Number of reflections of the new ray.</param>
    /// <returns> A new <c>Ray</c> originating from the interaction point.</returns>
    public abstract Ray ScatterRay(PCG pcg, Vector incidentVector, Point interactionPoint, Normal normal, int depth);
}

/// <summary>
/// <c>BRDF</c> in which all incoming radiation is distributed uniformly over the 2π hemisphere.
/// </summary>
public class DiffuseBRDF : BRDF
{
    private float _reflectance;

    /// <summary>
    /// Constructs a <c>DiffuseBRDF</c> with a uniform black <c>Pigment</c> and unity reflectance.
    /// </summary>
    public DiffuseBRDF() : base()
    {
        _reflectance = 1;
    }

    /// <summary>
    /// Constructs a <c>DiffuseBRDF</c> with a specified <c>Pigment</c> and unity reflectance.
    /// </summary>
    /// <param name="pigment">
    /// The pigment that defines the surface color.
    /// </param>
    public DiffuseBRDF(Pigment pigment) : base(pigment)
    {
        _reflectance = 1;
    }

    /// <summary>
    /// Constructs a <c>DiffuseBRDF</c> with a specified <c>Pigment</c> and reflectance.
    /// </summary>
    /// <param name="pigment">
    /// The pigment that defines the surface color.
    /// </param>
    /// <param name="reflectance">
    /// The reflectance coefficient of the surface.
    /// </param>
    public DiffuseBRDF(Pigment pigment, float reflectance) : base(pigment)
    {
        _reflectance = reflectance;
    }

    // da togliere?
    /* public override Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv)
     {
         return pigment.GetColor(uv) * reflectance * (1.0f / MathF.PI);
     }*/

    /// <summary>
    /// Returns a <c>Ray</c> randomly generated over the hemisphere oriented by the surface <c>normal</c>
    /// and originating at the <c>interactionPoint</c>.
    /// The new ray will have the provided <c>depth</c>.
    /// </summary>
    /// <param name="pcg"> Random number generator used for Monte Carlo sampling.</param>
    /// <param name="incidentVector">Direction of the incoming <c>Ray</c>.</param>
    /// <param name="interactionPoint"><c>Point</c> on the surface where the scattering event occurs.</param>
    /// <param name="normal"> Surface <c>Normal</c> used to orient the new ray.</param>
    /// <param name="depth"> Number of reflections of the new ray.</param>
    /// <returns> A new <c>Ray</c> originating from the interaction point and traveling in the sampled direction.</returns>
    public override Ray ScatterRay(PCG pcg, Vector incidentVector, Point interactionPoint, Normal normal, int depth)
    {
        Shape.CreateONB(normal, out Vector e1, out Vector e2, out Vector e3);

        // This algorithm uses the importance sampling, using the Phong distribution with n=1
        // (instead of generating uniformly on the hemisphere)
        // i.e. p(theta, phi) = cos(theta) sin(theta) / pi
        // With this distribution we generate theta and phi as follows
        float phi = 2 * MathF.PI * pcg.RandomFloat();
        float cosThetaSq = pcg.RandomFloat(); // i.e. theta = arccos(sqrt(y)) with y uniformly in (0,1)
        float cosTheta = MathF.Sqrt(cosThetaSq);
        float sinTheta = MathF.Sqrt(1 - cosThetaSq);

        return new Ray
        (
            interactionPoint,
            e1 * sinTheta * MathF.Cos(phi) + e2 * sinTheta * MathF.Sin(phi) + e3 * cosTheta,
            1e-03f,
            float.PositiveInfinity,
            depth
        );
    }
}

/// <summary>
/// <c>BRDF</c> in which the incoming radiation is reflected along the direction given by the reflection law.
/// </summary>
public class SpecularBRDF : BRDF
{
    /* public override Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv)
     {
         throw new NotImplementedException();
     }*/
    
    /// <summary>
    /// Constructs a <c>SpecularBRDF</c> with a uniform black <c>Pigment</c>
    /// </summary>
    public SpecularBRDF() : base()
    {
    }

    /// <summary>
    /// Constructs a <c>SpecularBRDF</c> with a specified <c>Pigment</c>
    /// </summary>
    /// <param name="pigment"> The pigment defining the surface color </param>
    public SpecularBRDF(Pigment pigment) : base(pigment)
    {
    }

    //da modificare
    /// <summary>
    /// Returns a <c>Ray</c> generated accordingly to the reflection's law and originating at the <c>interactionPoint</c>.
    /// The new ray will have the provided <c>depth</c>.
    /// </summary>
    /// <param name="pcg">Parameter not used.</param>
    /// <param name="incidentVector">Direction of the incoming <c>Ray</c>.</param>
    /// <param name="interactionPoint"><c>Point</c> on the surface where the scattering event occurs.</param>
    /// <param name="normal"> Surface <c>Normal</c> used to orient the new ray.</param>
    /// <param name="depth"> Number of reflections of the new ray.</param>
    /// <returns> A new <c>Ray</c> originating from the interaction point and traveling in the sampled direction.</returns>
    public override Ray ScatterRay(PCG pcg, Vector incidentVector, Point interactionPoint, Normal normal, int depth)
    {
        //Tomasi non ho capito perché normalizza il vettore incidente
        //Vector rayDir = new Vector(incindentVector.X, incindentVector.Y, incindentVector.Z);
        //rayDir.Normalize();
        Vector normalVec = normal.ToVector();

        return new Ray(
            interactionPoint,
            incidentVector - normalVec * 2 * (normalVec * incidentVector),
            1e-3f,
            float.PositiveInfinity,
            depth
        );
    }
}