using System.Globalization;

namespace TracerLib;

public abstract class Token
{
    public SourceLocation Location { get; }

    protected Token(SourceLocation location)
    {
        Location = location;
    }
}

public enum Keyword
{
    New,
    Material,
    Plane,
    Sphere,
    Diffuse,
    Specular,
    Uniform,
    Checkered,
    Image,
    //Identity,
    //Translation,
    RotationX,
    RotationY,
    RotationZ,
    Scaling,
    Camera,
    Orthogonal,
    Perspective,
    Float
}

public static class Keywords
{
    public static readonly Dictionary<string, Keyword> Keyword = new Dictionary<string, Keyword>
    {
        ["new"] = TracerLib.Keyword.New,
        ["material"] = TracerLib.Keyword.Material,
        ["plane"] = TracerLib.Keyword.Plane,
        ["sphere"] = TracerLib.Keyword.Sphere,
        ["diffuse"] = TracerLib.Keyword.Diffuse,
        ["specular"] = TracerLib.Keyword.Specular,
        ["uniform"] = TracerLib.Keyword.Uniform,
        ["checkered"] = TracerLib.Keyword.Checkered,
        ["image"] = TracerLib.Keyword.Image,
        ["rotation_x"] = TracerLib.Keyword.RotationX,
        ["rotation_y"] = TracerLib.Keyword.RotationY,
        ["rotation_z"] = TracerLib.Keyword.RotationZ,
        ["scaling"] = TracerLib.Keyword.Scaling,
        ["camera"] = TracerLib.Keyword.Camera,
        ["orthogonal"] = TracerLib.Keyword.Orthogonal,
        ["perspective"] = TracerLib.Keyword.Perspective,
        ["float"] = TracerLib.Keyword.Float
    };
}

//Cambiare in sealed anche tutte le altre classi che non verranno ereditate (orthogonal, sphere, etc)
public sealed class KeywordToken : Token
{
    public Keyword Keyword { get; }

    public KeywordToken(SourceLocation location, Keyword keyword) : base(location)
    {
        Keyword = keyword;
    }

    public override string ToString()
    {
        return Keyword.ToString();
    }
}

public sealed class IdentifierToken : Token
{
    public string Identifier { get; }

    public IdentifierToken(SourceLocation location, string identifier) : base(location)
    {
        Identifier = identifier;
    }

    public override string ToString()
    {
        return Identifier;
    }
}

public sealed class StringToken : Token
{
    public string String { get; }

    public StringToken(SourceLocation location, string s) : base(location)
    {
        String = s;
    }

    public override string ToString()
    {
        return String;
    }
}

public sealed class LiteralNumberToken : Token
{
    public float Value { get; }

    public LiteralNumberToken(SourceLocation location, float value) : base(location)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}

public sealed class SymbolToken : Token
{
    public string Symbol { get; }

    public SymbolToken(SourceLocation location, string symbol) : base(location)
    {
        Symbol = symbol;
    }

    public override string ToString()
    {
        return Symbol;
    }
}