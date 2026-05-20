namespace TracerLib;

public abstract class BRDF
{
    protected Pigment pigment;

    //public abstract Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv);

    public abstract Ray ScatterRay(PCG pcg, Vector Vin, Point interactionPoint, Normal normal, int depth);
}

public class DiffuseBRDF : BRDF
{
    private float reflectance;

    public DiffuseBRDF()
    {
        reflectance = 1;
    }

    public DiffuseBRDF(float reflectance)
    {
        this.reflectance = reflectance;
    }

   /* public override Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv)
    {
        return pigment.GetColor(uv) * reflectance * (1.0f / MathF.PI);
    }*/

    //da RIVEDERE PER LA GENERAZIONE DI THETA TRA 0 E PI/2
    public override Ray ScatterRay(PCG pcg, Vector Vin, Point interactionPoint, Normal normal, int depth)
    {
        Vector e1, e2, e3;
        Shape.CreateONB(normal, out e1, out e2, out e3);

        float phi = 2 * MathF.PI * pcg.RandomFloat();
        float cos_theta_sq = pcg.RandomFloat();
        float cos_theta = MathF.Sqrt(cos_theta_sq);
        float sin_theta = MathF.Sqrt(1 - cos_theta_sq);


        return new Ray
        (
            interactionPoint,
            e1 * sin_theta * MathF.Cos(phi) + e2 * sin_theta * MathF.Sin(phi) + e3 * cos_theta,
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
    public override Ray ScatterRay(PCG pcg, Vector Vin, Point interactionPoint, Normal normal, int depth)
    {
        Vector ray = new Vector(Vin.X, Vin.Y, Vin.Z);
        //ray.Normalize(); //Tomasi non ho capito perché normalizza il raggio
        Vector normal_vec = normal.ToVector();
        float phi = 2 * MathF.PI * pcg.RandomFloat();

        return new Ray(
            interactionPoint,
            ray - normal_vec * 2 * (normal_vec * ray),
            1e-3f,
            float.PositiveInfinity,
            depth
        );
    }
}