//using System.Diagnostics;

namespace TracerLib;
//namespace Colors; //questo lo mettiamo?

//rivedere dopo aver letto di try-except (per l'operatore *)
//forse è meglio passare by value per efficienza certi argomenti, meglio chiedere a Tomasi
//magari si può migliorare la classe usando i primary constructor?
public readonly struct Color
{
    private readonly float _R, _G, _B;

    public Color(float r, float g, float b)
    {
        _R = r;
        _G = g;
        _B = b;
    }

    /*
    public float R { get; set; } //Controllare metodo "get;"
    public float G { get; set; }
    public float B { get; set; }*/
    
    
    public static Color operator +(in Color c1, in Color c2)
    {
        return new Color(c1._R + c2._R, c1._G + c2._G, c1._B + c2._B);
    }
    
    public static Color operator *(in Color a, float alpha)
    {
        return new Color(a._R * alpha, a._G * alpha, a._B * alpha);
    }

    public static Color operator *(float b, in Color a)
    {
        return a * b;
    }
    
    //prodotto di Hadamard
    public static Color operator *(in Color c1, in Color c2)
    {
        return new Color(c1._R * c2._R, c1._G * c2._G, c1._B * c2._B);
    }

    public static bool AreSameColor(in Color c1, in Color c2)
    {
        return c1._R == c2._R
               && c1._G == c2._G
               && c1._B == c2._B;
    }

    public static bool _AreCloseColor(in Color c1, in Color c2, float epsilon = 1e-3f)
    {
        return Functions.AreClose(c1._R, c2._R, epsilon)
               && Functions.AreClose(c1._G, c2._G, epsilon)
               && Functions.AreClose(c1._B, c2._B, epsilon);
    }

    /*   public static bool _are_close(Color a, Color b, float epsilon = 1e-5f)
        {
            return MathF.Abs(a.R - b.R) < epsilon && MathF.Abs(a.G - b.G) < epsilon && MathF.Abs(a.B - b.B) < epsilon;
        }*/


       
    
    public void Print()
    {
        Console.Write($"({_R}, {_G}, {_B})");
    }


    // public bool _AreColorsValid(float R, float G, float B)
    // {
    //     return R >= 0 && G >= 0 && B >= 0;
    // }
    
    public float Luminosity() //RIVEDERE THIS
    {
        return (MathF.Max(MathF.Max(this._R, this._G), this._B) + MathF.Min(MathF.Min(this._R, this._G), this._B)) / 2;
    }
}
/*
string path = "/home/giacomo/prove_Csharp/Prove_Csharp/Prove_Csharp/prova";
//string path = "prova";
using (StreamReader sr = new StreamReader(path))
{
    while (sr.Peek() > -1)
    {
        Console.WriteLine(sr.Peek());
        Console.WriteLine(sr.ReadLine());
    }
}
*/
