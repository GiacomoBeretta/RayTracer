///
/// This file is release under ... license. See LICENSE.md
/// 

using System.Diagnostics;

namespace TracerLib;
//namespace Colors; //questo lo mettiamo?

//rivedere dopo aver letto di try-except (per l'operatore *)
//forse è meglio passare by value per efficienza certi argomenti, meglio chiedere a Tomasi
//magari si può migliorare la classe usando i primary constructor?

/// <summary>
/// A Color type is identified by 3 float positive values R,G,B.
/// Some basic implemented operations: sum, product of a color by a scalar, product between 2 colors.
/// </summary>
/* public readonly struct Color
{
    //private readonly float _R, _G, _B;

    public Color(float r, float g, float b)
    {
        _R = r;
        _G = g;
        _B = b;
    }

//non so se mettere anche i set, nel caso bisogna cambiare la classe che per ora è del tipo readonly
    public float R
    {
        get { return _R; }
    }

    public float G
    {
        get { return _G; }
    }

    public float B
    {
        get { return _B; }
    }
*/
public struct Color
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    
    public Color(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }
    
    public static Color operator +(in Color c1, in Color c2)
    {
        return new Color(c1.R + c2.R, c1.G + c2.G, c1.B + c2.B);
    }

    public static Color operator *(in Color a, float alpha)
    {
        return new Color(a.R * alpha, a.G * alpha, a.B * alpha);
    }

    public static Color operator *(float b, in Color a)
    {
        return a * b;
    }

    //prodotto di Hadamard
    public static Color operator *(in Color c1, in Color c2)
    {
        return new Color(c1.R * c2.R, c1.G * c2.G, c1.B * c2.B);
    }

    /// <summary>
    /// Returns if the 2 colors passed are exactly equal.
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns></returns>
    public static bool _AreSameColor(in Color c1, in Color c2)
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
    public static bool _AreCloseColor(in Color c1, in Color c2, float epsilon = 1e-5f)
    {
        return Functions.AreClose(c1.R, c2.R, epsilon)
               && Functions.AreClose(c1.G, c2.G, epsilon)
               && Functions.AreClose(c1.B, c2.B, epsilon);
    }

    /*   public static bool _are_close(Color a, Color b, float epsilon = 1e-5f)
        {
            return MathF.Abs(a.R - b.R) < epsilon && MathF.Abs(a.G - b.G) < epsilon && MathF.Abs(a.B - b.B) < epsilon;
        }*/

    public void Print()
    {
        Console.Write($"({R}, {G}, {B})");
    }

    // public bool _AreColorsValid(float R, float G, float B)
    // {
    //     return R >= 0 && G >= 0 && B >= 0;
    // }

    public float LuminosityShirleyMorley() //RIVEDERE THIS
    {
        return (MathF.Max(MathF.Max(R, G), B) + MathF.Min(MathF.Min(R, G), B)) / 2.0f;
    }

    public float LuminosityWeightedAverage()
    {
        float w_R = 0.2126f;
        float w_G = 0.7152f;
        float w_B = 0.0722f; //the weights sum to 1
        //return (w_R*R + w_G*G + w_B*B)/(w_R+w_G+w_B);
        return w_R * R + w_G * G + w_B * B;
    }

    /// <summary>
    /// returns the corresponding sRGB triple corrected by the characteristc gamma factor of the display
    /// </summary>
    /// <param name="gamma"></param>
    /// <returns></returns>
    public Color To8BitRGB(float gamma)
    {
        //Debug.Assert(float.IsPositive()); 
        float r = (float)Math.Round(255*MathF.Pow(R, 1.0f/gamma));
        float g = (float)Math.Round(255*MathF.Pow(G, 1.0f/gamma));
        float b = (float)Math.Round(255*MathF.Pow(B, 1.0f/gamma));

        return new Color(r,g,b);
    }
}