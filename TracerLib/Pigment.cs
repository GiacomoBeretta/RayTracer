// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

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

    //da commentare
    public override Color GetColor(Vector2D uv)
    {
        float u = uv.U - MathF.Floor(uv.U); // magari può essere utile scrivere mathf.Floor(uv.U-epsilon) così se
        // uv.U vale 1 viene fuori 1-math.floor(0.99) = 1-0 =1 (e non 1-1=0)
        float v = uv.V - MathF.Floor(uv.V); // idem per uv.V

        int col = (int)(u * this.Image.Width);
        int row = (int)(v * this.Image.Height);

        if (col >= Image.Width) col = Image.Width - 1;//come mai questo if?
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
        //Normalizzazione coordinate u,v [forza entrambi i valori nell'intervallo (0,1)](Per risolvere l'artefatto grafico nell'immagine)
        var u = uv.U - MathF.Floor(uv.U);
        var v = uv.V - MathF.Floor(uv.V);

        var iu = (int)(MathF.Floor(u * this.NumSteps));
        var iv = (int)(MathF.Floor(v * this.NumSteps));

        return ((iu % 2) == (iv % 2)) ? this.Color1 : this.Color2;
    }
}