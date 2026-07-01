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
    /// The Pigment property represents the texture of the surface.
    /// </summary>
    public Pigment Pigment;

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

    public virtual Color Eval(Normal normal, Vector vin, Vector vout, Vector2D uv)
    {
        return Pigment.GetColor(uv);
    }

    /// <summary>
    /// Returns a <see cref="Ray"/> generated according to the type of <see cref="BRDF"/>.
    /// The new ray will have the provided <paramref name="depth"/>.
    /// </summary>
    /// <param name="pcg"> Random number generator used for Monte Carlo sampling.</param>
    /// <param name="incidentVector">Direction of the incoming ray.</param>
    /// <param name="interactionPoint"><see cref="Point"/> on the surface where the scattering event occurs.</param>
    /// <param name="normal"> Surface <see cref="Normal"/> used to orient the new ray.</param>
    /// <param name="depth"> Number of reflections of the new ray.</param>
    /// <returns> A new <see cref="Ray"/> originating from the interaction point.</returns>
    public abstract Ray ScatterRay(PCG pcg, Vector incidentVector, Point interactionPoint, Normal normal, int depth);
}

/// <summary>
/// <see cref="BRDF"/> in which all incoming radiation is distributed uniformly over the 2π hemisphere.
/// </summary>
public class DiffuseBRDF : BRDF
{
    private float _reflectance;

    /// <summary>
    /// Constructs a <see cref="DiffuseBRDF"/> with a uniform black <see cref="Pigment"/> and unity reflectance.
    /// </summary>
    public DiffuseBRDF() : base()
    {
        _reflectance = 1;
    }

    /// <summary>
    /// Constructs a <see cref="DiffuseBRDF"/> with a specified <see cref="Pigment"/> and unity reflectance.
    /// </summary>
    /// <param name="pigment">
    /// The pigment that defines the surface color.
    /// </param>
    public DiffuseBRDF(Pigment pigment) : base(pigment)
    {
        _reflectance = 1;
    }

    /// <summary>
    /// Constructs a <see cref="DiffuseBRDF"/>with a specified <see cref="Pigment"/> and reflectance.
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

    public override Color Eval(Normal normal, Vector vin, Vector vout, Vector2D uv)
    {
        return Pigment.GetColor(uv) * _reflectance * (1.0f / MathF.PI);
    }

    /// <summary>
    /// Returns a <see cref="Ray"/> randomly generated over the hemisphere oriented by the surface <see cref="Normal"/>
    /// and originating at the <paramref name="interactionPoint"/>.
    /// The new ray will have the provided <paramref name="depth"/>.
    /// </summary>
    /// <param name="pcg"> Random number generator used for Monte Carlo sampling.</param>
    /// <param name="incidentVector">Direction of the incoming ray.</param>
    /// <param name="interactionPoint"><see cref="Point"/> on the surface where the scattering event occurs.</param>
    /// <param name="normal"> Surface <see cref="Normal"/> used to orient the new ray.</param>
    /// <param name="depth"> Number of reflections of the new ray.</param>
    /// <returns> A new <see cref="Ray"/> originating from the interaction point and traveling in the sampled direction.</returns>
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
/// <see cref="BRDF"/> in which the incoming radiation is reflected along the direction given by the reflection law.
/// </summary>
public class SpecularBRDF : BRDF
{
    public override Color Eval(Normal normal, Vector vin, Vector vout, Vector2D uv)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Constructs a <see cref="SpecularBRDF"/>with a uniform black <see cref="Pigment"/>.
    /// </summary>
    public SpecularBRDF() : base()
    {
    }

    /// <summary>
    /// Constructs a <see cref="SpecularBRDF"/> with a specified <see cref="Pigment"/>.
    /// </summary>
    /// <param name="pigment">The pigment defining the surface color.</param>
    public SpecularBRDF(Pigment pigment) : base(pigment)
    {
    }

    /// <summary>
    /// Returns a <see cref="Ray"/> generated accordingly to the reflection's law
    /// and originating at the <paramref name="interactionPoint"/>.
    /// The new ray will have the provided <paramref name="depth"/>.
    /// </summary>
    /// <param name="pcg">Parameter not used.</param>
    /// <param name="incidentVector">Direction of the incoming Ray.</param>
    /// <param name="interactionPoint"><see cref="Point"/> on the surface where the scattering event occurs.</param>
    /// <param name="normal"> Surface <see cref="Normal"/> used to orient the new ray.</param>
    /// <param name="depth"> Number of reflections of the new ray.</param>
    /// <returns> A new <see cref="Ray"/> originating from the interaction point and traveling in the sampled direction.</returns>
    public override Ray ScatterRay(PCG pcg, Vector incidentVector, Point interactionPoint, Normal normal, int depth)
    {
        // Maybe it could be advantageous normalize the incident vector before firing the new ray
        //
        //Vector rayDir = new Vector(incidentVector.X, incidentVector.Y, incidentVector.Z);
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