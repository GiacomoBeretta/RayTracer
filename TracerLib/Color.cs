// This file is release under EUPL_v1.2 license. See LICENSE.md

using System.Diagnostics.CodeAnalysis;

namespace TracerLib;

//forse un controllo sul prodotto per scalare è troppo?
//forse è meglio passare by value per efficienza certi argomenti, meglio chiedere a Tomasi
//magari si può migliorare lo struct usando i primary constructor?

/// <summary>
/// The Color type is identified by 3 float positive values R,G,B.
/// Some basic implemented operations: sum, product of a color by a scalar, product between 2 colors.
/// </summary>
public struct Color
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }

    /// <summary>
    /// Basic <see cref="Color"/> constructor which accepts 3 positive parameters between 0 and 1 : R,G,B 
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Color(float r, float g, float b)
    {
        if (r < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(r), r, nameof(r) + " must be non-negative");
        }

        if (g < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(g), g, nameof(g) + " must be non-negative");
        }

        if (b < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(b), b, nameof(b) + " must be non-negative");
        }

        R = r;
        G = g;
        B = b;
    }

    /// <summary>
    /// Sum per component between two <see cref="Color"/>
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns></returns>
    public static Color operator +(Color c1, Color c2)
    {
        return new Color(c1.R + c2.R, c1.G + c2.G, c1.B + c2.B);
    }

    /// <summary>
    /// Product between a <see cref="Color"/> and a float scalar
    /// </summary>
    /// <param name="a">Color</param>
    /// <param name="alpha">Scalar</param>
    /// <returns></returns>
    public static Color operator *(Color a, float alpha)
    {
        if (alpha < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(alpha), alpha, nameof(alpha) + " must be non-negative");
        }

        return new Color(a.R * alpha, a.G * alpha, a.B * alpha);
    }

    /// <summary>
    /// Product between a <see cref="Color"/> and a float scalar
    /// </summary>
    /// <param name="alpha">Scalar</param>
    /// <param name="a">Color</param>
    /// <returns></returns>
    public static Color operator *(float alpha, Color a)
    {
        return a * alpha;
    }

    /// <summary>
    /// Hadamard's product: Product oper component between two <see cref="Color"/> (used in RenderFunction).
    /// </summary>
    /// <param name="c1">First Color</param>
    /// <param name="c2">Second Color</param>
    /// <returns></returns>
    public static Color operator *(Color c1, Color c2)
    {
        return new Color(c1.R * c2.R, c1.G * c2.G, c1.B * c2.B);
    }

    /// <summary>
    /// Returns whether the 2 Colors passed are exactly equal.
    /// </summary>
    /// <param name="c1">First Color</param>
    /// <param name="c2">Second Color</param>
    /// <returns></returns>
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public static bool _AreSameColor(Color c1, Color c2)
    {
        return c1.R == c2.R
               && c1.G == c2.G
               && c1.B == c2.B;
    }

    /// <summary>
    /// Returns whether the 2 Colors passed are equal
    /// within a difference given by the epsilon parameter to deal with floating numbers.
    /// </summary>
    /// <param name="c1">First Color</param>
    /// <param name="c2">Second Color</param>
    /// <param name="epsilon">Epsilon parameter</param>
    /// <returns></returns>
    public static bool _AreColorsClose(Color c1, Color c2, float epsilon = 1e-5f)
    {
        return Functions.AreClose(c1.R, c2.R, epsilon)
               && Functions.AreClose(c1.G, c2.G, epsilon)
               && Functions.AreClose(c1.B, c2.B, epsilon);
    }

    /// <summary>
    /// Returns a formatted string with the RGB colors.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"(R={R}, G={G}, B={B})";
    }

    /// <summary>
    /// Prints the formatted string with RGB colors
    /// </summary>
    /// <returns></returns>
    public void Print()
    {
        Console.WriteLine(ToString());
    }

    // public bool _AreColorsValid(float R, float G, float B)
    // {
    //     return R >= 0 && G >= 0 && B >= 0;
    // }

    /// <summary>
    /// Returns the luminosity of a pixel using the formula given by Shirley and Morley 
    /// </summary>
    /// <returns></returns>
    public float LuminosityShirleyMorley()
    {
        return (MathF.Max(MathF.Max(R, G), B) + MathF.Min(MathF.Min(R, G), B)) / 2.0f;
    }

    /// <summary>
    /// Returns the luminosity of a pixel using the ITU-R BT.709 standard
    /// see https://en.wikipedia.org/wiki/Rec._709
    /// </summary>
    /// <returns></returns>
    public float LuminosityWeightedAverage() //VERIFICARE I PESI
    {
        const float wR = 0.2126f;
        const float wG = 0.7152f;
        const float wB = 0.0722f; //the weights sum to 1
        //return (w_R*R + w_G*G + w_B*B)/(w_R+w_G+w_B);
        return wR * R + wG * G + wB * B;
    }

    /// <summary>
    /// Maps a value to the range [0, 1),
    /// using the transformation x / (x + 1).
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static float _Clamp(float x)
    {
        return x / (x + 1);
    }

    /// <summary>
    /// Applies the function x / (x + 1)
    /// to each RGB component,
    /// reducing the intensity of too bright colors.
    /// </summary>
    public void _Clamp()
    {
        R = Color._Clamp(R);
        G = Color._Clamp(G);
        B = Color._Clamp(B);
    }

    /// <summary>
    /// Applies gamma correction using a power-law function
    /// and maps the resulting color to the 0–255 range.
    /// </summary>
    /// <param name="gamma">Gamma exponent used for power-law correction (must be > 0).
    /// It's characteristic of the display used.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when gamma is less than or equal to 0.
    /// </exception>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public Color To8BitRGB(float gamma)
    {
        if (gamma <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(gamma), gamma, nameof(gamma) + " must be greater than 0");
        }

        float r = (float)Math.Round(255 * MathF.Pow(R, 1.0f / gamma));
        float g = (float)Math.Round(255 * MathF.Pow(G, 1.0f / gamma));
        float b = (float)Math.Round(255 * MathF.Pow(B, 1.0f / gamma));

        return new Color(r, g, b);
    }
}