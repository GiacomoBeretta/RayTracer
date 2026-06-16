// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// Represents the optical properties of a surface material,
/// including its reflection model and any emitted radiance.
/// </summary>
/// <remarks>
/// A material is defined by a <see cref="BRDF"/>, which describes
/// how incoming light is reflected, and an optional emitted radiance,
/// allowing the material to act as a light source.
/// </remarks>
public struct Material
{
    /// <summary>
    /// Gets the pigment describing the surface reflectance properties of the material.
    /// </summary>
    public Pigment Pigment => Brdf.Pigment;

    /// <summary>
    /// The radiance emitted by the surface material, varying over the surface.
    /// </summary>
    public Pigment EmittedRadiance;
    
    /// <summary>
    /// The bidirectional reflectance distribution function that
    /// describes how light is reflected by the material.
    /// </summary>
    public BRDF Brdf;

    /// <summary>
    /// Initializes a non-emissive material with the specified BRDF.
    /// </summary>
    /// <param name="brdf">
    /// The reflection model used by the material.
    /// </param>
    public Material(BRDF brdf)
    {
        Brdf = brdf;
        Color black = new Color(0, 0, 0);
        EmittedRadiance = new UniformPigment(black);
    }

    /// <summary>
    /// Initializes a material with the specified emitted radiance
    /// and reflection model.
    /// </summary>
    /// <param name="emittedRadiance">
    /// The radiance emitted by the material.
    /// </param>
    /// <param name="brdf">
    /// The reflection model used by the material.
    /// </param>
    public Material(Pigment emittedRadiance, BRDF brdf)
    {
        EmittedRadiance = emittedRadiance;
        Brdf = brdf;
    }
}