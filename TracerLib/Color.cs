// This file is release under EUPL_v1.2 license. See LICENSE.md

using System.Diagnostics.CodeAnalysis;

namespace TracerLib;

//rivedere dopo aver letto di try-except (per l'operatore *)
//forse è meglio passare by value per efficienza certi argomenti, meglio chiedere a Tomasi
//magari si può migliorare la classe usando i primary constructor?

/// <summary>
/// A Color type is identified by 3 float positive values R,G,B.
/// Some basic implemented operations: sum, product of a color by a scalar, product between 2 colors.
/// </summary>
public struct Color
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }

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

    public static Color operator +(Color c1, Color c2)
    {
        return new Color(c1.R + c2.R, c1.G + c2.G, c1.B + c2.B);
    }

    public static Color operator *(Color a, float alpha)
    {
        return new Color(a.R * alpha, a.G * alpha, a.B * alpha);
    }

    public static Color operator *(float alpha, Color a)
    {
        return a * alpha;
    }

    // Hadamard's Product
    public static Color operator *(Color c1, Color c2)
    {
        return new Color(c1.R * c2.R, c1.G * c2.G, c1.B * c2.B);
    }

    /// <summary>
    /// Returns if the 2 colors passed are exactly equal.
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns></returns>
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public static bool _AreSameColor(Color c1, Color c2)
    {
        return c1.R == c2.R
               && c1.G == c2.G
               && c1.B == c2.B;
    }

    /// <summary>
    /// Returns if the 2 colors passed are equal within a difference given by the epsilon parameter to deal with floating numbers.
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <param name="epsilon"></param>
    /// <returns></returns>
    public static bool _AreColorsClose(Color c1, Color c2, float epsilon = 1e-5f)
    {
        return Functions.AreClose(c1.R, c2.R, epsilon)
               && Functions.AreClose(c1.G, c2.G, epsilon)
               && Functions.AreClose(c1.B, c2.B, epsilon);
    }

/*   public static bool _are_close(Color a, Color b, float epsilon = 1e-5f)
    {
        return MathF.Abs(a.R - b.R) < epsilon && MathF.Abs(a.G - b.G) < epsilon && MathF.Abs(a.B - b.B) < epsilon;
    }*/

    /// <summary>
    /// Return a formatted string with the RGB colors.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"({R}, {G}, {B})";
    }

    public void Print()
    {
        Console.WriteLine(ToString());
    }

// public bool _AreColorsValid(float R, float G, float B)
// {
//     return R >= 0 && G >= 0 && B >= 0;
// }

    /// <summary>
    /// Returns the luminosity of a pixel using the formula given by Shirley and Morley in their book
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
    /// Clamps the value of x under 1
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static float _Clamp(float x)
    {
        return x / (x + 1);
    }

    /// <summary>
    /// Clamps the values of RGB under 1
    /// and resize a potential too bright pixel
    /// </summary>
    public void _Clamp()
    {
        R = Color._Clamp(R);
        G = Color._Clamp(G);
        B = Color._Clamp(B);
    }

    /// <summary>
    /// Returns the corresponding sRGB triple corrected by the characteristic gamma factor of the display
    /// </summary>
    /// <param name="gamma"></param>
    /// <returns></returns>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public Color To8BitRGB(float gamma)
    {
        float r = (float)Math.Round(255 * MathF.Pow(R, 1.0f / gamma));
        float g = (float)Math.Round(255 * MathF.Pow(G, 1.0f / gamma));
        float b = (float)Math.Round(255 * MathF.Pow(B, 1.0f / gamma));

        return new Color(r, g, b);
    }
    
}