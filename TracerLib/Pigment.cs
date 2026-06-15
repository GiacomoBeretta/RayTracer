// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

//non ho capito se le coordinate uv sono tra 0 e 1 o possono essere più grandi o addirittura negative

/// <summary>
/// A base class representing the texture of a surface.
/// </summary>
public abstract class Pigment
{
    /// <summary>
    /// Returns the color evaluated at the given uv coordinates.
    /// </summary>
    /// <param name="uv">Surface texture coordinates expressed as a <see cref="Vector2D"/>.</param>
    /// <returns>The evaluated <see cref="Color"/> at the specified uv position.</returns>
    public abstract Color GetColor(Vector2D uv);
}

/// <summary>
/// A <see cref="Pigment"/> that provides a single uniform color over all the surface.
/// </summary>
public class UniformPigment : Pigment
{
    public Color Color { get; }

    public UniformPigment(Color color)
    {
        Color = color;
    }

    public override Color GetColor(Vector2D uv)
    {
        return Color;
    }
}

/// <summary>
/// A <see cref="Pigment"/> that provides a texture based on an image.
/// </summary>
public class ImagePigment : Pigment
{
    public HDRImage Image { get; }

    public ImagePigment(HDRImage image)
    {
        Image = image;
    }

    //forse qui c'è un errore
    /// <summary>
    /// Returns the color sampled from the image at the given UV coordinates.
    /// UV coordinates are defined such that (0,0) corresponds to the top-left
    /// corner of the <see cref="HDRImage"/>, consistent with its row/column indexing.
    /// </summary>
    /// <param name="uv">
    /// Surface texture coordinates expressed as a <see cref="Vector2D"/>.
    /// </param>
    /// <returns>
    /// The <see cref="Color"/> sampled from the image at the specified UV position.
    /// </returns>
    public override Color GetColor(Vector2D uv)
    {
        float u = uv.U - MathF.Floor(uv.U); // magari può essere utile scrivere mathf.Floor(uv.U-epsilon) così se
        // uv.U vale 1 viene fuori 1-math.floor(0.99) = 1-0 =1 (e non 1-1=0)
        float v = uv.V - MathF.Floor(uv.V); // idem per uv.V

        int col = (int)(u * Image.Width);
        int row = (int)(v * Image.Height);

        if (col >= Image.Width) col = Image.Width - 1; //come mai questo if?
        if (row >= Image.Height) row = Image.Height - 1;

        return Image[col, row];
    }
}

/// <summary>
/// A <see cref="Pigment"/> that generates a checkerboard pattern using two colors.
/// </summary>
public class CheckeredPigment : Pigment
{
    public Color Color1 { get; }
    public Color Color2 { get; }

    /// <summary>
    /// Number of subdivisions per axis in UV space used to generate the checker pattern.
    /// For example, a value of 4 produces a 4x4 checker grid.
    /// </summary>
    public int NumSteps { get; }

    public CheckeredPigment(Color color1, Color color2, int numsteps = 10)
    {
        Color1 = color1;
        Color2 = color2;
        NumSteps = numsteps;
    }

    //forse qui c'è un errore
    /// <summary>
    /// Returns one of the two colors of the checkered pattern based on the given uv coordinates.
    /// </summary>
    /// <param name="uv">
    /// Surface texture coordinates expressed as a <see cref="Vector2D"/>.
    /// </param>
    /// <returns>
    /// The <see cref="Color"/> of the checker pattern at the specified UV position.
    /// </returns>
    public override Color GetColor(Vector2D uv)
    {
        // Normalizzazione coordinate u,v [forza entrambi i valori nell'intervallo (0,1)](Per risolvere l'artefatto grafico nell'immagine)
        // questo serve credo per colorare anche i piani infiniti, cioè per quando U e V sono più grandi di 1.
        float u = uv.U - MathF.Floor(uv.U);
        float v = uv.V - MathF.Floor(uv.V);
        // Now u,v are in (0,1)

        // e.g. if NumSteps = 4, u = 0.6, v = 0.1 then
        // iu = Floor(0.6 * 4)= Floor(2.4) = 2
        // iv = Floor(0.1 * 4) = Floor(0.4) = 0
        // so (iu, iv) indicates the third cell of the first line that must have the first color
        int iu = (int)(MathF.Floor(u * NumSteps));
        int iv = (int)(MathF.Floor(v * NumSteps));

        //return (iu + iv) % 2 == 0 ? Color1 : Color2; //magari è più veloce
        return ((iu % 2) == (iv % 2)) ? Color1 : Color2;
    }
}