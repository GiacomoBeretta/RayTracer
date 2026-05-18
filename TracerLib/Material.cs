namespace TracerLib;

//magari è meglio definire Pigment come un delegate (cioè un function object)

public abstract class Pigment
{
    public abstract Color GetColor(Vector2D uv);
}

public class UniformPigment : Pigment
{
    public Color Color { get; }

    public UniformPigment(Color color)
    {
        Color = color;
    }

    public override Color GetColor(Vector2D uv)
    {
        return this.Color;
    }
}

public class ImagePigment : Pigment
{
    public HDRImage Image { get; }

    public ImagePigment(HDRImage image)
    {
        Image = image;
    }

    public override Color GetColor(Vector2D uv)
    {
        var col = (int)(uv.U * this.Image.Width);
        var row = (int)(uv.V * this.Image.Height);

        if (col >= Image.Width) col = Image.Width - 1;
        if (row >= Image.Height) row = Image.Height - 1;

        return Image[col, row];
    }
}

public class CheckeredPigment : Pigment
{
    public Color Color1 { get; }
    public Color Color2 { get; }
    public int NumSteps { get; }

    public CheckeredPigment(Color color1, Color color2, int numsteps = 10)
    {
        Color1 = color1;
        Color2 = color2;
        NumSteps = numsteps;
    }

    public override Color GetColor(Vector2D uv)
    {
        var iu = (int)(MathF.Floor(uv.U * this.NumSteps));
        var iv = (int)(MathF.Floor(uv.V * this.NumSteps));

        return ((iu % 2) == (iv % 2)) ? this.Color1 : this.Color2;
    }
}

public abstract class BRDF
{
    protected Pigment pigment;

    public abstract Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv);

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

    public override Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv)
    {
        return pigment.GetColor(uv) * reflectance * (1.0f / MathF.PI);
    }

    //da modificare
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
            new Vector(),
            tmin: 1e-03f,
            depth: depth
        );
    }
}

public class SpecularBRDF : BRDF
{
    public override Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv)
    {
        throw new NotImplementedException();
    }

    //da modificare
    public override Ray ScatterRay(PCG pcg, Vector Vin, Point interactionPoint, Normal normal, int depth)
    {
        Vector e1, e2, e3;
        Shape.CreateONB(normal, out e1, out e2, out e3);
        float phi = 2 * MathF.PI * pcg.RandomFloat();

        return new Ray();
    }
}

public struct Material
{
    public Pigment Pigment;
    public Pigment EmittedRadiance;
    public BRDF Brdf;

    public Material(Pigment pigment, BRDF brdf)
    {
        this.Pigment = pigment;
        this.Brdf = brdf;
        Color Black = new Color(0, 0, 0);
        this.EmittedRadiance = new UniformPigment(Black);
    }

    public Material(Pigment pigment, Pigment emittedRadiance, BRDF brdf)
    {
        this.Pigment = pigment;
        this.EmittedRadiance = emittedRadiance;
        this.Brdf = brdf;
    }
}