// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

//Verificare se modificare {token} in {type(token)} nei messaggi d'errore cambia qualcosa
//Considerare di cambiare ExpectSymbol(InputStream, string) in ExpecteSymbol(InputStream, char)

/// <summary>
/// A class that serves for interpret the txt file that contains the informations of the scene to render.
/// </summary>
public class Scene
{
    public Dictionary<string, Material> Materials { get; set; } = new();
    public World World { get; set; } = new();
    public ICamera? Camera { get; set; } = null;
    public Dictionary<string, float> Variables { get; set; } = new();
    public HashSet<string> OverriddenVariables { get; set; } = [];

    /// <summary>
    /// Reads the next token from the input stream and verifies that it matches the expected symbol.
    /// </summary>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <param name="symbol">The expected symbol value to match against the next token.</param>
    /// <exception cref="SceneSyntaxException">Thrown when the next token is not a symbol token or its value does not match the expected symbol.</exception>
    public void ExpectSymbol(InputStream inputStream, string symbol)
    {
        Token token = inputStream.ReadNextToken();
        if (token is not SymbolToken symbolToken || symbolToken.Symbol != symbol)
        {
            throw new SceneSyntaxException(token.Location, $"expected {symbol} but got {token}");
        }
    }

    /// <summary>
    /// Reads the next token from the input stream and ensures it is a KeywordToken and that its value
    /// is included in the provided <paramref name="keywords"/> list.
    /// </summary>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <param name="keywords">The list of valid keywords that are allowed at this position.</param>
    /// <returns>The parsed <see cref="Keyword"/> value.</returns>
    /// <exception cref="SceneSyntaxException">
    ///  Thrown when the next token is not a keyword, or when the keyword is not contained in <paramref name="keywords"/>.
    /// </exception>
    public Keyword ExpectKeyword(InputStream inputStream, List<Keyword> keywords)
    {
        Token token = inputStream.ReadNextToken();

        if (token is not KeywordToken keywordToken)
        {
            throw new SceneSyntaxException(token.Location, $"expected keyword instead of {token}");
        }

        if (!keywords.Contains(keywordToken.Keyword))
        {
            throw new SceneSyntaxException(token.Location,
                $"expected one of the following keywords: {string.Join(", ", keywords)} instead of {token}");
        }

        return keywordToken.Keyword;
    }

    /// <summary>
    /// Reads the next token from the input stream and evaluates it as a numeric value.
    /// The token can be either a numeric literal or a variable identifier.
    /// </summary>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <returns>The numeric value represented by the next token.</returns>
    /// <exception cref="SceneSyntaxException">
    /// Thrown when the token is not a number or a valid variable reference,
    /// or when the referenced variable is not defined in <see cref="Variables"/>.
    /// </exception>
    public float ExpectNumber(InputStream inputStream)
    {
        Token token = inputStream.ReadNextToken();

        if (token is LiteralNumberToken literalNumberToken) return literalNumberToken.Value;

        if (token is IdentifierToken identifierToken)
        {
            string variableName = identifierToken.Identifier;

            if (!Variables.ContainsKey(variableName))
                throw new SceneSyntaxException(token.Location, $"unknow variable {token}");

            return Variables[variableName];
        }

        throw new SceneSyntaxException(token.Location, $"expected a number instead of {token}");
    }

    /// <summary>
    /// Reads the next token from the input stream and ensures it is a StringToken.
    /// </summary>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <returns>The string value of the parsed token.</returns>
    /// <exception cref="SceneSyntaxException">
    /// Thrown if the next token is not a string literal.
    /// </exception>
    public string ExpectString(InputStream inputStream)
    {
        Token token = inputStream.ReadNextToken();

        if (token is not StringToken stringToken)
            throw new SceneSyntaxException(token.Location, $"expected a literal string instead of {token}");

        return stringToken.String;
    }
    
    /// <summary>
    /// Reads the next token from the input stream and ensures it is an IdentifierToken.
    /// </summary>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <returns>The identifier string from the next token.</returns>
    /// <exception cref="SceneSyntaxException">
    /// Thrown when the next token is not an identifier.
    /// </exception>
    public string ExpectIdentifier(InputStream inputStream)
    {
        Token token = inputStream.ReadNextToken();

        if (token is not IdentifierToken identifierToken)
            throw new SceneSyntaxException(token.Location, $"expected an identifier instead of {token}");

        return identifierToken.Identifier;
    }

    /// <summary>
    /// Parses a three-dimensional vector from the input stream.
    /// </summary>
    /// <remarks>
    /// Expected grammar:
    /// vector ::= "[" number "," number "," number "]"
    /// where each component is either a numeric literal or a valid variable reference.
    /// </remarks>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <returns>A <see cref="Vector"/> containing the parsed coordinates.</returns>
    public Vector ParseVector(InputStream inputStream)
    {
        ExpectSymbol(inputStream, "[");
        float x = ExpectNumber(inputStream);
        ExpectSymbol(inputStream, ",");
        float y = ExpectNumber(inputStream);
        ExpectSymbol(inputStream, ",");
        float z = ExpectNumber(inputStream);
        ExpectSymbol(inputStream, "]");

        return new Vector(x, y, z);
    }
    
    /// <summary>
    /// Parses an RGB Color from the input stream.
    /// </summary>
    /// <remarks>
    /// Expected grammar:
    /// <code>
    /// color ::= "&lt;" number "," number "," number "&gt;"
    /// </code>
    /// </remarks>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <returns>A <see cref="Color"/> containing the parsed RGB values.</returns>
    public Color ParseColor(InputStream inputStream)
    {
        ExpectSymbol(inputStream, "<");
        float r = ExpectNumber(inputStream);
        ExpectSymbol(inputStream, ",");
        float g = ExpectNumber(inputStream);
        ExpectSymbol(inputStream, ",");
        float b = ExpectNumber(inputStream);
        ExpectSymbol(inputStream, ">");

        return new Color(r, g, b);
    }
    
    /// <summary>
    /// Parses a <see cref="Pigment"/> from the input stream.
    /// Expected grammar:
    /// <code>
    /// pigment ::= uniform_pigment | checkered_pigment | image_pigment
    /// uniform_pigment ::= "uniform" "(" color ")"
    /// checkered_pigment ::= "checkered" "(" color "," color "," number ")"
    /// image_pigment ::= "image" "(" LITERAL_STRING ")"
    /// </code>
    /// </summary>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <returns>
    /// A <see cref="Pigment"/> instance representing the parsed pigment.
    /// </returns>
    /// <exception cref="SceneSyntaxException">Thrown when the keyword is not among the expected.</exception>
    public Pigment ParsePigment(InputStream inputStream)
    {
        Keyword keyword = ExpectKeyword(inputStream, [Keyword.Uniform, Keyword.Checkered, Keyword.Image]);
        Pigment result;

        ExpectSymbol(inputStream, "(");

        switch (keyword)
        {
            case Keyword.Uniform:
                Color color = ParseColor(inputStream);
                result = new UniformPigment(color);
                break;
            case Keyword.Checkered:
                Color color1 = ParseColor(inputStream);
                ExpectSymbol(inputStream, ",");
                Color color2 = ParseColor(inputStream);
                ExpectSymbol(inputStream, ",");
                int steps = (int)ExpectNumber(inputStream);
                result = new CheckeredPigment(color1, color2, steps);
                break;
            case Keyword.Image:
                string fileName = ExpectString(inputStream);
                string currentPath = AppDomain.CurrentDomain.BaseDirectory;
                string texturePath = Path.Combine(currentPath, "../../../../PfmImages", fileName);
                using (FileStream imageFile = File.OpenRead(texturePath))
                {
                    HDRImage image = HDRImage.ReadPFM_File(imageFile);
                    result = new ImagePigment(image);
                }
                break;
            default:
                throw new SceneSyntaxException(inputStream.Location, "Keyword doesn't match any of the Pigments types");
        }

        ExpectSymbol(inputStream, ")");
        return result;
    }
    
    /// <summary>
    /// Parses a BRDF definition from the input stream.
    /// Expected grammar:
    /// <code>
    /// brdf ::= diffuse_brdf | specular_brdf
    ///
    /// diffuse_brdf  ::= "diffuse" "(" pigment ")"
    /// specular_brdf ::= "specular" "(" pigment ")"
    /// </code>
    /// </summary>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <returns>
    /// A <see cref="BRDF"/> instance representing the parsed reflectance model.
    /// </returns>
    /// <exception cref="SceneSyntaxException">
    /// Thrown when the input does not match a valid BRDF definition or contains invalid tokens.
    /// </exception>
    public BRDF ParseBRDF(InputStream inputStream)
    {
        Keyword BRDFkeyword = ExpectKeyword(inputStream, [Keyword.Diffuse, Keyword.Specular]);
        ExpectSymbol(inputStream, "(");
        Pigment pigment = ParsePigment(inputStream);
        ExpectSymbol(inputStream, ")");
        BRDF result;

        switch (BRDFkeyword)
        {
            case Keyword.Diffuse:
                result = new DiffuseBRDF(pigment);
                break;
            case Keyword.Specular:
                result = new SpecularBRDF(pigment);
                break;
            default:
                throw new SceneSyntaxException(inputStream.Location, "Keyword doesn't match any of the BRDF types");
        }

        return result;
    }

    public void ParseMaterial(InputStream inputStream, out string name, out Material material)
    {
        name = ExpectIdentifier(inputStream);

        ExpectSymbol(inputStream, "(");
        BRDF brdf = ParseBRDF(inputStream);
        ExpectSymbol(inputStream, ",");
        Pigment emittedRadiance = ParsePigment(inputStream);
        ExpectSymbol(inputStream, ")");

        material = new Material(emittedRadiance, brdf);
    }

    public Transformation ParseTransformation(InputStream inputStream)
    {
        var result = new Transformation();

        while (true)
        {
            Keyword transformKeyword = ExpectKeyword(inputStream,
            [
                Keyword.Identity, Keyword.Translation, Keyword.RotationX, Keyword.RotationY, Keyword.RotationZ,
                Keyword.Scaling
            ]);

            switch (transformKeyword)
            {
                case Keyword.Identity:
                    break;
                case Keyword.Translation:
                    ExpectSymbol(inputStream, "(");
                    result *= new Transformation(ParseVector(inputStream));
                    ExpectSymbol(inputStream, ")");
                    break;
                case Keyword.RotationX:
                    ExpectSymbol(inputStream, "(");
                    float degx = ExpectNumber(inputStream);
                    result *= new Transformation(Axis.X, Functions.DegToRad(degx));
                    ExpectSymbol(inputStream, ")");
                    break;
                case Keyword.RotationY:
                    ExpectSymbol(inputStream, "(");
                    float degy = ExpectNumber(inputStream);
                    result *= new Transformation(Axis.Y, Functions.DegToRad(degy));
                    ExpectSymbol(inputStream, ")");
                    break;
                case Keyword.RotationZ:
                    ExpectSymbol(inputStream, "(");
                    float degz = ExpectNumber(inputStream);
                    result *= new Transformation(Axis.Z, Functions.DegToRad(degz));
                    ExpectSymbol(inputStream, ")");
                    break;
                case Keyword.Scaling:
                    ExpectSymbol(inputStream, "(");
                    float x = ExpectNumber(inputStream);
                    ExpectSymbol(inputStream, ",");
                    float y = ExpectNumber(inputStream);
                    ExpectSymbol(inputStream, ",");
                    float z = ExpectNumber(inputStream);
                    result *= new Transformation(x, y, z);
                    ExpectSymbol(inputStream, ")");
                    break;
                default:
                    throw new SceneSyntaxException(inputStream.Location,
                        "Keyword doesn't match any of the Transformation types");
            }

            Token nextToken = inputStream.ReadNextToken();
            if (nextToken is not SymbolToken symbolToken || symbolToken.Symbol != "*")
            {
                inputStream.UnreadToken(nextToken);
                break;
            }
        }

        return result;
    }

    public Sphere ParseSphere(InputStream inputStream, Scene scene)
    {
        ExpectSymbol(inputStream, "(");

        string material = ExpectIdentifier(inputStream);

        if (!Materials.ContainsKey(material))
            throw new SceneSyntaxException(inputStream.Location, $"unknown material {material}");

        ExpectSymbol(inputStream, ",");
        Transformation transformation = scene.ParseTransformation(inputStream);
        ExpectSymbol(inputStream, ")");

        return new Sphere(transformation, Materials[material]);
    }

    public Plane ParsePlane(InputStream inputStream, Scene scene)
    {
        ExpectSymbol(inputStream, "(");

        string material = ExpectIdentifier(inputStream);

        if (!Materials.ContainsKey(material))
            throw new SceneSyntaxException(inputStream.Location, $"unknown material {material}");

        ExpectSymbol(inputStream, ",");
        Transformation transformation = scene.ParseTransformation(inputStream);
        ExpectSymbol(inputStream, ")");

        return new Plane(transformation, Materials[material]);
    }

    public ICamera ParseCamera(InputStream inputStream, Scene scene)
    {
        ExpectSymbol(inputStream, "(");
        Keyword cameraKeyword = ExpectKeyword(inputStream, [Keyword.Perspective, Keyword.Orthogonal]);
        ExpectSymbol(inputStream, ",");
        Transformation transformation = scene.ParseTransformation(inputStream);
        ExpectSymbol(inputStream, ",");
        float aspectRatio = ExpectNumber(inputStream);
        ExpectSymbol(inputStream, ",");
        float distance = ExpectNumber(inputStream);
        ExpectSymbol(inputStream, ")");

        ICamera result;

        switch (cameraKeyword)
        {
            case Keyword.Perspective:
                result = new PerspectiveCamera(transformation, distance, aspectRatio);
                break;
            case Keyword.Orthogonal:
                result = new OrthogonalCamera(transformation, aspectRatio);
                break;
            default:
                throw new SceneSyntaxException(inputStream.Location, "Keyword doesn't match any of the Cameras types");
        }

        return result;
    }

    public Scene ParseScene(InputStream inputStream, Dictionary<string, float> variables)
    {
        var scene = new Scene
        {
            Variables = new Dictionary<string, float>(variables),
            OverriddenVariables = new HashSet<string>(variables.Keys)
        };

        while (true)
        {
            Token token = inputStream.ReadNextToken();
            if (token is StopToken) break;
            if (token is not KeywordToken)
                throw new SceneSyntaxException(token.Location, $"expected keyword instead of {token}");
            if (token is KeywordToken { Keyword: Keyword.Float })
            {
                string variableName = ExpectIdentifier(inputStream);

                SourceLocation variableLocation = inputStream.Location;

                ExpectSymbol(inputStream, "(");
                float variableValue = ExpectNumber(inputStream);
                ExpectSymbol(inputStream, ")");

                if (scene.Variables.ContainsKey(variableName) && !scene.OverriddenVariables.Contains(variableName))
                    throw new SceneSyntaxException(variableLocation,
                        $"{variableName} cannot be redefined");
                if (!scene.OverriddenVariables.Contains(variableName)) scene.Variables[variableName] = variableValue;
            }
            else if (token is KeywordToken { Keyword: Keyword.Sphere })
            {
                var sphere = scene.ParseSphere(inputStream, scene);
                scene.World.Add(sphere);
            }
            else if (token is KeywordToken { Keyword: Keyword.Plane })
            {
                scene.World.Add(scene.ParsePlane(inputStream, scene));
            }
            else if (token is KeywordToken { Keyword: Keyword.Camera })
            {
                if (scene.Camera != null) throw new SceneSyntaxException(token.Location, "Cannot define more Cameras");

                scene.Camera = scene.ParseCamera(inputStream, scene);
            }
            else if (token is KeywordToken { Keyword: Keyword.Material })
            {
                scene.ParseMaterial(inputStream, out string name, out Material material);
                scene.Materials[name] = material;
            }
        }

        return scene;
    }
}