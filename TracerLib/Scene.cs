// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

//Verificare se modificare {token} in {type(token)} nei messaggi d'errore cambia qualcosa

/// <summary>
/// Represents a scene parsed from a scene description file.
/// </summary>
public class Scene
{
    // Properties - Begin

    #region Properties

    /// <summary>
    /// Maps material identifiers to the corresponding material definitions.
    /// </summary>
    public Dictionary<string, Material> Materials { get; set; } = new();

    /// <summary>
    /// The set of all the objects of the scene.
    /// </summary>
    public World World { get; set; } = new();

    /// <summary>
    /// The camera used to render the scene.
    /// See <see cref="Camera"/> for more information.
    /// </summary>
    public ICamera? Camera { get; set; } = null;

    /// <summary>
    /// Maps variable names to their values.
    /// </summary>
    public Dictionary<string, float> Variables { get; set; } = new();

    /// <summary>
    /// Names of variables provided externally (e.g., via command-line arguments).
    /// External variables take precedence over values defined in the scene file.
    /// </summary>
    /// <remarks>
    /// The scene file must still declare all variables it uses to remain self-consistent.
    /// This set contains only variable names, not their values.
    /// </remarks>
    public HashSet<string> ExternalVariables { get; set; } = [];

    #endregion

    // Properties - End

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
    /// <code>
    /// vector ::= "[" number "," number "," number "]"
    /// </code>
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
    /// </summary>
    /// <remarks>
    /// Expected grammar:
    /// <code>
    /// pigment ::= uniform_pigment | checkered_pigment | image_pigment
    /// uniform_pigment ::= "uniform" "(" color ")"
    /// checkered_pigment ::= "checkered" "(" color "," color "," number ")"
    /// image_pigment ::= "image" "(" LITERAL_STRING ")"
    /// </code>
    /// </remarks>
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
    /// </summary>
    /// <remarks>
    /// Expected grammar:
    /// <code>
    /// brdf ::= diffuse_brdf | specular_brdf
    /// diffuse_brdf  ::= "diffuse" "(" pigment ")"
    /// specular_brdf ::= "specular" "(" pigment ")"
    /// </code>
    /// </remarks>
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

    /// <summary>
    /// Parses a material definition from the input stream.
    /// </summary>
    /// <remarks>
    /// Expected grammar:
    /// <code>
    /// material ::= "material" IDENTIFIER "(" brdf "," pigment ")"
    /// </code>
    /// </remarks>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <param name="name">
    /// The identifier name associated with the parsed material.
    /// </param>
    /// <param name="material">
    /// The resulting <see cref="Material"/> instance constructed from the parsed data.
    /// </param>
    /// <exception cref="SceneSyntaxException">
    /// Thrown when the input does not match the expected material definition syntax.
    /// </exception>
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

    /// <summary>
    /// Parses a sequence of transformations from the input stream.
    /// </summary>
    /// <remarks>
    /// Transformations are evaluated in order and combined using composition.
    /// Expected grammar:
    /// <code>
    /// transformation ::= basic_transformation | basic_transformation "*" transformation
    /// basic_transformation ::= "identity"
    ///           | "translation" "(" vector ")"
    ///           | "rotation_x" "(" number ")"
    ///           | "rotation_y" "(" number ")"
    ///           | "rotation_z" "(" number ")"
    ///           | "scaling" "(" number "," number "," number ")"
    /// </code>
    /// where angles are expressed in degrees and converted internally to radians.
    /// </remarks>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <returns>
    /// A <see cref="Transformation"/> representing the composed transformation chain.
    /// </returns>
    /// <exception cref="SceneSyntaxException">
    /// Thrown when the input does not match the expected transformation grammar.
    /// </exception>
    public Transformation ParseTransformation(InputStream inputStream)
    {
        Transformation result = new Transformation();

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
            if (nextToken is SymbolToken { Symbol: "*" })
            {
                continue;
            }

            inputStream.UnreadToken(nextToken);
            break;
        }

        return result;
    }

    /// <summary>
    /// Parses a sphere definition from the input stream.
    /// </summary>
    /// <remarks>
    /// Expected grammar:
    /// <code>
    /// sphere_decl ::= "sphere" "(" IDENTIFIER "," transformation ")"
    /// </code>
    /// </remarks>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <returns>
    /// A <see cref="Sphere"/> initialized with the specified material and transformation.
    /// </returns>
    /// <exception cref="SceneSyntaxException">
    /// Thrown when the input does not match the expected sphere grammar or when the referenced material is undefined.
    /// </exception>
    public Sphere ParseSphere(InputStream inputStream)
    {
        ExpectSymbol(inputStream, "(");

        string material = ExpectIdentifier(inputStream);

        if (!Materials.ContainsKey(material))
            throw new SceneSyntaxException(inputStream.Location, $"unknown material {material}");

        ExpectSymbol(inputStream, ",");
        Transformation transformation = ParseTransformation(inputStream);
        ExpectSymbol(inputStream, ")");

        return new Sphere(transformation, Materials[material]);
    }

    /// <summary>
    /// Parses a plane definition from the input stream.
    /// </summary>
    /// <remarks>
    /// Expected grammar:
    /// <code>
    /// plane_decl ::= "plane" "(" IDENTIFIER "," transformation ")"
    /// </code>
    /// </remarks>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <returns>
    /// A <see cref="Plane"/> initialized with the specified material and transformation.
    /// </returns>
    /// <exception cref="SceneSyntaxException">
    /// Thrown when the input does not match the expected plane grammar or when the referenced material is undefined.
    /// </exception>
    public Plane ParsePlane(InputStream inputStream)
    {
        ExpectSymbol(inputStream, "(");

        string material = ExpectIdentifier(inputStream);

        if (!Materials.ContainsKey(material))
            throw new SceneSyntaxException(inputStream.Location, $"unknown material {material}");

        ExpectSymbol(inputStream, ",");
        Transformation transformation = ParseTransformation(inputStream);
        ExpectSymbol(inputStream, ")");

        return new Plane(transformation, Materials[material]);
    }

    /// <summary>
    /// Parses a camera definition from the input stream.
    /// </summary>
    /// <remarks>
    /// Expected grammar:
    /// camera_decl ::= "camera" "(" camera_type "," camera_params ")"
    /// camera_type ::= "orthogonal" | "perspective"
    /// camera_params ::= orthogonal_params | perspective_params
    /// orthogonal_params ::= transformation "," number "," number
    /// perspective_params ::= transformation "," number "," number "," number
    /// </remarks>
    /// <param name="inputStream">The input stream providing tokens to read.</param>
    /// <returns>
    /// An <see cref="ICamera"/> instance of the appropriate type
    /// (<see cref="PerspectiveCamera"/> or <see cref="OrthogonalCamera"/>).
    /// </returns>
    /// <exception cref="SceneSyntaxException">
    /// Thrown if the input does not match the expected syntax or if the camera type is invalid.
    /// </exception>
    public ICamera ParseCamera(InputStream inputStream)
    {
        ICamera result;
        
        ExpectSymbol(inputStream, "(");
        Keyword cameraKeyword = ExpectKeyword(inputStream, [Keyword.Perspective, Keyword.Orthogonal]);
        ExpectSymbol(inputStream, ",");
        Transformation transformation = ParseTransformation(inputStream);
        ExpectSymbol(inputStream, ",");
        float width = ExpectNumber(inputStream);
        ExpectSymbol(inputStream, ",");
        float height = ExpectNumber(inputStream);
        
        switch (cameraKeyword)
        {
            case Keyword.Perspective:
                ExpectSymbol(inputStream, ",");
                float distance = ExpectNumber(inputStream);
                result = new PerspectiveCamera(transformation, width, height, distance);
                break;
            case Keyword.Orthogonal:
                result = new OrthogonalCamera(transformation, width, height);
                break;
            default:
                throw new SceneSyntaxException(inputStream.Location, "Keyword doesn't match any of the Cameras types");
        }
        ExpectSymbol(inputStream, ")");
        
        return result;
    }

    /// <summary>
    /// Registers a variable definition from the scene file.
    /// External variables take precedence over scene-defined values.
    /// </summary>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="variableValue">Value defined in the scene file.</param>
    /// <param name="variableLocation">Location used for error reporting.</param>
    /// <exception cref="SceneSyntaxException">
    /// Thrown when a variable is defined more than once in the scene file.
    /// </exception>
    public void RegisterVariable(string variableName, float variableValue, SourceLocation variableLocation)
    {
        if (Variables.ContainsKey(variableName) && !ExternalVariables.Contains(variableName))
            throw new SceneSyntaxException(variableLocation,
                $"{variableName} cannot be redefined");
        if (!ExternalVariables.Contains(variableName)) Variables[variableName] = variableValue;
    }

    /// <summary>
    /// Populates this <see cref="Scene"/> by parsing a scene description from the input stream,
    /// taking into account external variables that override values defined in the scene file.
    /// </summary>
    /// <param name="inputStream">
    /// The input stream providing the scene description.
    /// </param>
    /// <param name="externalVariables">
    /// Variables provided externally whose values take precedence over
    /// those defined in the scene file.
    /// </param>
    /// <exception cref="SceneSyntaxException">
    /// Thrown when the scene description violates the syntax rules,
    /// attempts to redefine a variable, or defines more than one camera.
    /// </exception>
    public void ReadScene(InputStream inputStream, Dictionary<string, float> externalVariables)
    {
        Variables = new Dictionary<string, float>(externalVariables);
        ExternalVariables = new HashSet<string>(externalVariables.Keys);

        while (true)
        {
            Token token = inputStream.ReadNextToken();

            if (token is StopToken) break;

            switch (token)
            {
                case KeywordToken { Keyword: Keyword.Float }:
                    string variableName = ExpectIdentifier(inputStream);

                    SourceLocation variableLocation = inputStream.Location;

                    ExpectSymbol(inputStream, "(");
                    float variableValue = ExpectNumber(inputStream);
                    ExpectSymbol(inputStream, ")");

                    RegisterVariable(variableName, variableValue, variableLocation);
                    break;
                case KeywordToken { Keyword: Keyword.Sphere }:
                    Sphere sphere = ParseSphere(inputStream);
                    World.Add(sphere);
                    break;
                case KeywordToken { Keyword: Keyword.Plane }:
                    Plane plane = ParsePlane(inputStream);
                    World.Add(plane);
                    break;
                case KeywordToken { Keyword: Keyword.Camera }:
                    if (Camera != null) throw new SceneSyntaxException(token.Location, "Cannot define more Cameras");
                    Camera = ParseCamera(inputStream);
                    break;
                case KeywordToken { Keyword: Keyword.Material }:
                    ParseMaterial(inputStream, out string name, out Material material);
                    Materials[name] = material;
                    break;
                default:
                    throw new SceneSyntaxException(token.Location, $"expected keyword instead of {token}");
            }
        }
    }
}