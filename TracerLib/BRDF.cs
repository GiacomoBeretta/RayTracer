// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// A class representing the Bidirectional Reflectance Distribution Function.
/// That is the ratio between the radiance leavnig a surface and the irradiance recieved
/// </summary>
public abstract class BRDF
{
    /// The Pigment property represents the texture of the surface
    public Pigment Pigment;

    // forse questo si può togliere?
    //public abstract Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv);
    protected BRDF() : this(new UniformPigment(new Color(0f,0f,0f))) {}

    protected BRDF(Pigment pigment)
    {
        Pigment = pigment;
    }
    
    //public abstract Ray ScatterRay(PCG pcg, Ray incidentRay, Point interactionPoint, Normal normal);
    public abstract Ray ScatterRay(PCG pcg, Vector incidentVector, Point interactionPoint, Normal normal, int depth);
}

/// <summary>
/// <c>BRDF</c> in which all incoming radiation is distributed over the 2π hemisphere
/// </summary>
public class DiffuseBRDF : BRDF
{
    private float _reflectance;

    /// <summary>
    /// Initialize a <c>DiffuseBRDF</c> with a uniform black <c>Pigment</c> and unity reflectance
    /// </summary>
    public DiffuseBRDF()
    {
        _reflectance = 1;
    }

    /// <summary>
    /// Initialize a <c>DiffuseBRDF</c> with a specified <c>Pigment</c> and unity reflectance
    /// </summary>
    /// <param name="pigment">
    /// The pigment define the surface color
    /// </param>
    public DiffuseBRDF(Pigment pigment) : base(pigment)
    {
        _reflectance = 1;
    }
    
    /// <summary>
    /// Initialize a <c>DiffuseBRDF</c> with a specified <c>Pigment</c> and reflectance 
    /// </summary>
    /// <param name="pigment">
    /// The pigment define the surface color
    /// </param>
    /// <param name="reflectance">
    /// The reflectance coefficient of the surface
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
    /// Generate a scattered <c>Ray</c> accordingly to the <c>DiffuseBRDF</c>
    /// </summary>
    /// <param name="pcg"> Random number generator for Monte Carlo sampling </param>
    /// <param name="vin"> Direction of the incoming <c>Ray</c> </param>
    /// <param name="interactionPoint"> Surface <c>Point</c> where the scattering event occur </param>
    /// <param name="normal"> Surface <c>Normal</c> </param>
    /// <param name="depth"> Recursion depth of the ray tracing process </param>
    /// <returns> A new <c>Ray</c> originating from the interaction point and traveling in the sampled direction</returns>
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
/// <c>BRDF</c> in which the incoming radiation is reflected along the direction given by the reflection law
/// </summary>
public class SpecularBRDF : BRDF
{
   /* public override Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv)
    {
        throw new NotImplementedException();
    }*/
   
   
   /// <summary>
   /// Initialize a <c>SpecularBRDF</c> with a uniform black <c>Pigment</c>
   /// </summary>
   public SpecularBRDF() {}
   
   /// <summary>
   /// Initialize a <c>SpecularBRDF</c> with a specified <c>Pigment</c>
   /// </summary>
   /// <param name="pigment"> The pigment defining the surface color </param>
   public SpecularBRDF(Pigment pigment) : base(pigment){}

    //da modificare
    /// <summary>
    /// Generate a scattered <c>Ray</c> accordingly to the reflection's law
    /// </summary>
    /// <param name="pcg"> Random number generator. This parameter won't be used because specular reflection is deterministic</param>
    /// <param name="vin">Direction of the incoming <c>Ray</c></param>
    /// <param name="interactionPoint">Surface <c>Point</c> where the reflection occurs</param>
    /// <param name="normal">Surface <c>Normal</c> at the interaction point</param>
    /// <param name="depth">Current recursion depth of the ray tracing process</param>
    /// <returns>A <c>Ray</c> originating from the interaction point and traveling in the mirror-reflected direction</returns>
    public override Ray ScatterRay(PCG pcg, Vector incindentVector, Point interactionPoint, Normal normal, int depth)
    {
        Vector rayDir = new Vector(incindentVector.X, incindentVector.Y, incindentVector.Z);
        //rayDir.Normalize(); //Tomasi non ho capito perché normalizza il vettore
        Vector normalVec = normal.ToVector();

        return new Ray(
            interactionPoint,
            rayDir - normalVec * 2 * (normalVec * rayDir),
            1e-3f,
            float.PositiveInfinity,
            depth
        );
    }
}