namespace TracerLib;

//magari è meglio definire Pigment come un delegate (cioè un function object)

public struct Material
{
    public Pigment Pigment => Brdf.Pigment;
    public Pigment EmittedRadiance;
    public BRDF Brdf;

    public Material(BRDF brdf)
    {
        this.Brdf = brdf;
        Color black = new Color(0, 0, 0);
        this.EmittedRadiance = new UniformPigment(black);
    }

    public Material(Pigment emittedRadiance, BRDF brdf)
    {
        this.EmittedRadiance = emittedRadiance;
        this.Brdf = brdf;
    }
}