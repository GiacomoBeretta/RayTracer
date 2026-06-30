// This file is release under EUPL_v1.2 license. See LICENSE.md

using System.Globalization;

namespace TracerLib;

/// <summary>
/// Represents the base class for all token types.
/// </summary>
public abstract class Token
{
    public SourceLocation Location { get; }

    protected Token(SourceLocation location)
    {
        Location = location;
    }

    public Token(string fileName, int line, int column)
    {
        Location = new SourceLocation(fileName, line, column);
    }

    public abstract override string ToString();
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
    Identity,
    Translation,
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
    public static readonly Dictionary<string, Keyword> Map = new Dictionary<string, Keyword>
    {
        ["new"] = Keyword.New,
        ["material"] = Keyword.Material,
        ["plane"] = Keyword.Plane,
        ["sphere"] = Keyword.Sphere,
        ["diffuse"] = Keyword.Diffuse,
        ["specular"] = Keyword.Specular,
        ["uniform"] = Keyword.Uniform,
        ["checkered"] = Keyword.Checkered,
        ["image"] = Keyword.Image,
        ["identity"] = Keyword.Identity,
        ["translation"] = Keyword.Translation,
        ["rotation_x"] = Keyword.RotationX,
        ["rotation_y"] = Keyword.RotationY,
        ["rotation_z"] = Keyword.RotationZ,
        ["scaling"] = Keyword.Scaling,
        ["camera"] = Keyword.Camera,
        ["orthogonal"] = Keyword.Orthogonal,
        ["perspective"] = Keyword.Perspective,
        ["float"] = Keyword.Float
    };
}

/// <summary>
/// Represents a token that signals the end of a file.
/// </summary>
public sealed class StopToken : Token
{
    public StopToken(SourceLocation location) : base(location)
    {
    }

    public override string ToString()
    {
        return $"{typeof(StopToken)}: a token to signal the end of file";
    }
}

/// <summary>
/// Represents a token that corresponds to a reserved keyword in the language.
/// </summary>
public sealed class KeywordToken : Token
{
    public Keyword Keyword { get; }

    public KeywordToken(SourceLocation location, Keyword keyword) : base(location)
    {
        Keyword = keyword;
    }

    public override string ToString()
    {
        return $"{typeof(KeywordToken)}: " + Keyword.ToString();
    }
}

/// <summary>
/// Represents an identifier token.
/// </summary>
public sealed class IdentifierToken : Token
{
    public string Identifier { get; }

    public IdentifierToken(SourceLocation location, string identifier) : base(location)
    {
        Identifier = identifier;
    }

    public override string ToString()
    {
        return $"{typeof(IdentifierToken)}: " + Identifier;
    }
}

/// <summary>
/// Represents a string literal token.
/// </summary>
public sealed class StringToken : Token
{
    public string String { get; }

    public StringToken(SourceLocation location, string s) : base(location)
    {
        String = s;
    }

    public override string ToString()
    {
        return $"{typeof(StringToken)}: " + String;
    }
}

/// <summary>
/// Represents a 32-bit floating-point literal.
/// </summary>
public sealed class LiteralNumberToken : Token
{
    public float Value { get; }

    public LiteralNumberToken(SourceLocation location, float value) : base(location)
    {
        Value = value;
    }

    public override string ToString()
    {
        return $"{typeof(LiteralNumberToken)}: " + Value.ToString(CultureInfo.InvariantCulture);
    }
}

/// <summary>
/// Represents a symbol token, such as '(', ')', '&lt;', or '+'.
/// </summary>
public sealed class SymbolToken : Token
{
    public string Symbol { get; }

    public SymbolToken(SourceLocation location, string symbol) : base(location)
    {
        Symbol = symbol;
    }

    public override string ToString()
    {
        return $"{typeof(SymbolToken)}: " + Symbol;
    }
}