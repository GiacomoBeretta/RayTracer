namespace TracerLib;

namespace TracerLib;

//magari è meglio definire Pigment come un delegate (cioè un function object)

public abstract class Pigment
{
    public Color GetColor(Vector2D uv)
    {
        return new Color(); //da modificare
    }
}

public class UniformPigment : Pigment
{

}

public class CheckeredPigment : Pigment
{

}

public class ImagePigment : Pigment
{

}

public abstract class BRDF
{
    protected Pigment pigment;

    public abstract Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv);
}

public class DiffuseBRDF : BRDF
{
    private float reflectance;
    public override Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv)
    {
        return pigment.GetColor(uv) * reflectance * (1.0f/ MathF.PI);
    }
}

public class SpecularBRDF : BRDF
{
    public override Color Eval(Normal normal, Vector Vin, Vector Vout, Vector2D uv)
    {
        throw new NotImplementedException();
    }
}

public struct Material
{
    Pigment pigment;
    BRDF brdf;

    public Material(Pigment pigment, BRDF brdf)
    {
        
    }
    
  //  public Pigment EmittedRadiance()
}