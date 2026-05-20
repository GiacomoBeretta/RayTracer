namespace TracerLib;

public abstract class BRDF
{
    protected Pigment pigment;

    //public abstract Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv);

    public abstract Ray ScatterRay(PCG pcg, Vector vin, Point interactionPoint, Normal normal, int depth);
}

public class DiffuseBRDF : BRDF
{
    private float _reflectance;

    public DiffuseBRDF()
    {
        _reflectance = 1;
    }

    public DiffuseBRDF(float reflectance)
    {
        this._reflectance = reflectance;
    }

   /* public override Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv)
    {
        return pigment.GetColor(uv) * reflectance * (1.0f / MathF.PI);
    }*/

    //da RIVEDERE PER LA GENERAZIONE DI THETA TRA 0 E PI/2
    public override Ray ScatterRay(PCG pcg, Vector vin, Point interactionPoint, Normal normal, int depth)
    {
        Shape.CreateONB(normal, out Vector e1, out Vector e2, out Vector e3);

        float phi = 2 * MathF.PI * pcg.RandomFloat();
        float cosThetaSq = pcg.RandomFloat();
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

public class SpecularBRDF : BRDF
{
   /* public override Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv)
    {
        throw new NotImplementedException();
    }*/

    //da modificare
    public override Ray ScatterRay(PCG pcg, Vector vin, Point interactionPoint, Normal normal, int depth)
    {
        Vector rayDir = new Vector(vin.X, vin.Y, vin.Z);
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