namespace TracerLib;

//magari è meglio definire Pigment come un delegate (cioè un function object)

public struct Material
{
    public Pigment Pigment;
    public Pigment EmittedRadiance;
    public BRDF Brdf;

    public Material(Pigment pigment, BRDF brdf)
    {
        this.Pigment = pigment;
        this.Brdf = brdf;
        Color black = new Color(0, 0, 0);
        this.EmittedRadiance = new UniformPigment(black);
    }

    public Material(Pigment pigment, Pigment emittedRadiance, BRDF brdf)
    {
        this.Pigment = pigment;
        this.EmittedRadiance = emittedRadiance;
        this.Brdf = brdf;
    }
}