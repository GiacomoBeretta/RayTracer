namespace Colors;

public struct Color
{
    public Color(float r, float g, float b) //Creare una variabile color Color() crea un colore nullo
    {
        R = r;
        G = g;
        B = b;
    }
    public float R { get; set; } //Controllare metodo "get;"
    public float G { get; set; }
    public float B { get; set; }

    public static Color operator +(Color a, Color b) //Controllare metodo static
    {
        return new Color(a.R + b.R, a.G + b.G, a.B + b.B); 
    }

    public static Color operator *(Color a, Color b) //Ricontrolla definizione prodotto tensore
    {
        return new Color(a.R * b.R, a.G * b.G, a.B * b.B);
    }

    public static Color operator *(Color a, float alpha)
    {
        return new Color(a.R * alpha, a.G * alpha, a.B * alpha);
    }
    
    public static Color operator *(float alpha, Color a)
    {
        return a * alpha;
    }

    public static bool _are_close(Color a, Color b, float epsilon = 1e-5f)
    {
        return MathF.Abs(a.R - b.R) < epsilon && MathF.Abs(a.G - b.G) < epsilon && MathF.Abs(a.B - b.B) < epsilon;
    }

    public float Luminosity() //RIVEDERE THIS
    {
        return (MathF.Max(MathF.Max(this.R, this.G), this.B) + MathF.Min(MathF.Min(this.R, this.G), this.B)) / 2;
    }
    
}