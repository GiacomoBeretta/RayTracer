// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// A base class representing the texture of a surface.
/// </summary>
public abstract class Pigment
{
    /// <summary>
    /// Evaluates the pigment at the given UV coordinates and returns its resulting color.
    /// </summary>
    /// <param name="uv">Surface texture coordinates expressed as a <see cref="Vector2D"/>, expected in the range [0, 1].</param>
    /// <returns>The evaluated <see cref="Color"/> at the specified uv position.</returns>
    public abstract Color GetColor(Vector2D uv);
}

/// <summary>
/// A <see cref="Pigment"/> that provides a single uniform color over all the surface.
/// </summary>
public class UniformPigment : Pigment
{
    /// <summary>
    /// The uniform color of this pigment.
    /// </summary>
    public Color Color { get; }

    /// <summary>
    /// Constructs a new instance of the <see cref="UniformPigment"/> class,
    /// with a uniform black color.
    /// </summary>
    public UniformPigment()
    {
        this.Color = new Color(0, 0, 0);
    }

    public UniformPigment(Color color)
    {
        Color = color;
    }

    /// <summary>
    /// Returns the uniform color of this pigment regardless of the input UV coordinates.
    /// </summary>
    /// <param name="uv">Ignored.</param>
    /// <returns>The uniform <see cref="Color"/> of this pigment.</returns>
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
    /// <summary>
    /// The texture image used to determine the pigment's color at each point.
    /// </summary>
    public HDRImage Image { get; }

    public ImagePigment(HDRImage image)
    {
        Image = image;
    }

    //TOGLIERE MATHFLOOR SUL BRANCH AVERAGEIMAGE
    /// <summary>
    /// Returns the color sampled from the image at the specified UV coordinates.
    /// </summary>
    /// <param name="uv">
    /// Surface texture coordinates expressed as a <see cref="Vector2D"/>.
    /// UV coordinates are defined such that (0,0) corresponds to the top-left
    /// corner of the <see cref="HDRImage"/>, consistent with its row/column indexing.
    /// Both U and V are expected to be in the range [0,1].
    /// </param>
    /// <returns>
    /// The <see cref="Color"/> sampled from the image at the specified UV coordinates.
    /// </returns>
    public override Color GetColor(Vector2D uv)
    {
        int col = (int)(uv.U * Image.Width);
        int row = (int)(uv.V * Image.Height);
        // now col is in [0, Image.Width] but the image is indexed with col in [0, Image.Width - 1]
        // similarly for row

        if (col >= Image.Width) col = Image.Width - 1;
        if (row >= Image.Height) row = Image.Height - 1;

        return Image[col, row];
    }
}

/// <summary>
/// A <see cref="Pigment"/> that generates a checkerboard pattern from two colors.
/// </summary>
public class CheckeredPigment : Pigment
{
    /// <summary>
    /// The first color used by this <see cref="CheckeredPigment"/>.
    /// </summary>
    public Color Color1 { get; }

    /// <summary>
    /// The second color used by this <see cref="CheckeredPigment"/>.
    /// </summary>
    public Color Color2 { get; }

    /// <summary>
    /// Number of subdivisions along each UV axis used to generate the checker pattern.
    /// For example, a value of 4 produces a 4x4 checker grid.
    /// </summary>
    public int NumSteps { get; }

    /// <summary>
    /// Constructs a new instance of the <see cref="CheckeredPigment"/> class.
    /// </summary>
    /// <param name="color1">The first color of the checkerboard pattern.</param>
    /// <param name="color2">The second color of the checkerboard pattern.</param>
    /// <param name="numsteps">Number of subdivisions along each UV axis used to generate the checker pattern. Defaults to 10.</param>
    public CheckeredPigment(Color color1, Color color2, int numsteps = 10)
    {
        Color1 = color1;
        Color2 = color2;
        NumSteps = numsteps;
    }

    /// <summary>
    /// Returns one of the two colors of the checkered pattern based on the given uv coordinates.
    /// </summary>
    /// <param name="uv">
    /// Surface texture coordinates expressed as a <see cref="Vector2D"/>, expected in the range [0, 1].
    /// </param>
    /// <returns>
    /// The <see cref="Color"/> corresponding to the checkerboard cell at the given UV position.
    /// </returns>
    public override Color GetColor(Vector2D uv)
    {
        // Convert UV coordinates into discrete grid cell indices iu and iv
        // e.g. if NumSteps = 4, U = 0.6, V = 0.1 then
        // iu = Floor(0.6 * 4)= Floor(2.4) = 2
        // iv = Floor(0.1 * 4) = Floor(0.4) = 0
        // so (iu, iv) indicates the third cell of the first line that must have the first color
        int iu = (int)(MathF.Floor(uv.U * NumSteps));
        int iv = (int)(MathF.Floor(uv.V * NumSteps));

        return (iu + iv) % 2 == 0 ? Color1 : Color2; //magari è più veloce
        //return ((iu % 2) == (iv % 2)) ? Color1 : Color2;
    }
}