// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

//Verificare se modificare {token} in {type(token)} nei messaggi d'errore cambia qualcosa
//Considerare di cambiare ExpectSymbol(InputStream, string) in ExpecteSymbol(InputStream, char)

/// <summary>
/// A class that serves for interpret the txt file that contains the informations of the scene to render.
/// </summary>
public class Scene
{
    public Dictionary<string, Material> Materials { get; set; } = new Dictionary<string, Material>();
    public World World { get; set; } = new();
    public ICamera? Camera { get; set; } = null;
    public Dictionary<string, float> Variables { get; set; } = new();
    //Overridden variables???

    /// <summary>
    /// Reads the next token and validates that it is a symbol token that contains the symbol expected.
    /// </summary>
    /// <param name="inputFile"></param>
    /// <param name="symbol">The symbol expected.</param>
    /// <exception cref="SceneSyntaxException"></exception>
    public void ExpectSymbol(InputStream inputFile, string symbol)
    {
        Token token = inputFile.ReadNextToken();
        if (token is not SymbolToken symbolToken || symbolToken.Symbol != symbol)
        {
            throw new SceneSyntaxException(token.Location, $"expected {symbol} but got {token}");
        }
    }

    public Keyword ExpectKeywords(InputStream inputFile, List<Keyword> keywords)
    {
        Token token = inputFile.ReadNextToken();

        if (token is not KeywordToken keywordToken)
        {
            throw new SceneSyntaxException(token.Location, $"expected keyword instead of {token}");
        }
        
        if (!keywords.Contains(keywordToken.Keyword))
        {
            throw new SceneSyntaxException(token.Location,
                $"expect one of the following keywords: {string.Join(", ", keywords)} instead of {token}");
        }

        return keywordToken.Keyword;
    }

    public float ExpectNumber(InputStream inputFile)
    {
        Token token = inputFile.ReadNextToken();

        if (token is LiteralNumberToken literalNumberToken) return literalNumberToken.Value;
        else if (token is IdentifierToken identifierToken)
        {
            string variableName = identifierToken.Identifier;
            if (!Variables.ContainsKey(variableName))
                throw new SceneSyntaxException(token.Location, $"unknow variable {token}");
            return Variables[variableName];
        }

        throw new SceneSyntaxException(token.Location, $"got {token} instead of a number");
    }

    public string ExpectString(InputStream inputFile)
    {
        Token token = inputFile.ReadNextToken();

        if (token is not StringToken stringToken)
            throw new SceneSyntaxException(token.Location, $"got {token} instead of a string");

        return stringToken.String;
    }

    public string ExpectIdentifier(InputStream inputFile)
    {
        Token token = inputFile.ReadNextToken();

        if (token is not IdentifierToken identifierToken)
            throw new SceneSyntaxException(token.Location, $"got {token} instead of an identifier");

        return identifierToken.Identifier;
    }

    public Vector ParseVector(InputStream inputFile)
    {
        ExpectSymbol(inputFile, "[");
        float x = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ",");
        float y = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ",");
        float z = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, "]");

        return new Vector(x, y, z);
    }

    public Color ParseColor(InputStream inputFile)
    {
        ExpectSymbol(inputFile, "<");
        float r = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ",");
        float g = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ",");
        float b = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ">");

        return new Color(r, g, b);
    }

    public Pigment ParsePigment(InputStream inputFile)
    {
        Keyword keyword = ExpectKeywords(inputFile, [Keyword.Uniform, Keyword.Checkered, Keyword.Image]);
        Pigment result;

        ExpectSymbol(inputFile, "(");

        switch (keyword)
        {
            case Keyword.Uniform:
                Color color = ParseColor(inputFile);
                result = new UniformPigment(color);
                break;
            case Keyword.Checkered:
                Color color1 = ParseColor(inputFile);
                ExpectSymbol(inputFile, ",");
                Color color2 = ParseColor(inputFile);
                ExpectSymbol(inputFile, ",");
                int steps = (int)ExpectNumber(inputFile);
                result = new CheckeredPigment(color1, color2, steps);
                break;
            case Keyword.Image:
                string fileName = ExpectString(inputFile);
                using (FileStream imageFile = File.OpenRead(fileName))
                {
                    HDRImage image = HDRImage.ReadPFM_File(imageFile);
                    result = new ImagePigment(image);
                }

                break;
            default:
                throw new SceneSyntaxException(inputFile.Location, "Keyword doesn't match any of the Pigments types");
        }

        ExpectSymbol(inputFile, ")");
        return result;
    }

    public BRDF ParseBRDF(InputStream inputFile)
    {
        Keyword BRDFkeyword = ExpectKeywords(inputFile, [Keyword.Diffuse, Keyword.Specular]);
        ExpectSymbol(inputFile, "(");
        Pigment pigment = ParsePigment(inputFile);
        ExpectSymbol(inputFile, ")");
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
                throw new SceneSyntaxException(inputFile.Location, "Keyword doesn't match any of the BRDF types");
        }

        return result;
    }

    public void ParseMaterial(InputStream inputFile, out string name, out Material material)
    {
        name = ExpectIdentifier(inputFile);

        ExpectSymbol(inputFile, "(");
        BRDF brdf = ParseBRDF(inputFile);
        ExpectSymbol(inputFile, ",");
        Pigment emittedRadiance = ParsePigment(inputFile);
        ExpectSymbol(inputFile, ")");

        material = new Material(emittedRadiance, brdf);
    }

    public Transformation ParseTransformation(InputStream inputFile)
    {
        var result = new Transformation();

        while (true)
        {
            Keyword transformKeyword = ExpectKeywords(inputFile,
            [
                Keyword.Identity, Keyword.Translation, Keyword.RotationX, Keyword.RotationY, Keyword.RotationZ,
                Keyword.Scaling
            ]);

            switch (transformKeyword)
            {
                case Keyword.Identity:
                    break;
                case Keyword.Translation:
                    ExpectSymbol(inputFile, "(");
                    result *= new Transformation(ParseVector(inputFile));
                    ExpectSymbol(inputFile, ")");
                    break;
                case Keyword.RotationX:
                    ExpectSymbol(inputFile, "(");
                    float degx = ExpectNumber(inputFile);
                    result *= new Transformation(Axis.X, Functions.DegToRad(degx));
                    ExpectSymbol(inputFile, ")");
                    break;
                case Keyword.RotationY:
                    ExpectSymbol(inputFile, "(");
                    float degy = ExpectNumber(inputFile);
                    result *= new Transformation(Axis.Y, Functions.DegToRad(degy));
                    ExpectSymbol(inputFile, ")");
                    break;
                case Keyword.RotationZ:
                    ExpectSymbol(inputFile, "(");
                    float degz = ExpectNumber(inputFile);
                    result *= new Transformation(Axis.Z, Functions.DegToRad(degz));
                    ExpectSymbol(inputFile, ")");
                    break;
                case Keyword.Scaling:
                    ExpectSymbol(inputFile, "(");
                    float x = ExpectNumber(inputFile);
                    ExpectSymbol(inputFile, ",");
                    float y = ExpectNumber(inputFile);
                    ExpectSymbol(inputFile, ",");
                    float z = ExpectNumber(inputFile);
                    result *= new Transformation(x, y, z);
                    ExpectSymbol(inputFile, ")");
                    break;
                default:
                    throw new SceneSyntaxException(inputFile.Location, "Keyword doesn't match any of the Transformation types");
            }

            Token nextToken = inputFile.ReadNextToken();
            if (nextToken is not SymbolToken symbolToken || symbolToken.Symbol != "*")
            {
                inputFile.UnreadToken(nextToken);
                break;
            }
        }

        return result;
    }

    public Sphere ParseSphere(InputStream inputFile, Scene scene)
    {
        ExpectSymbol(inputFile, "(");

        string material = ExpectIdentifier(inputFile);
        
        if (!Materials.ContainsKey(material))
            throw new SceneSyntaxException(inputFile.Location, $"unknown material {material}");

        ExpectSymbol(inputFile, ",");
        Transformation transformation = scene.ParseTransformation(inputFile);
        ExpectSymbol(inputFile, ")");

        return new Sphere(transformation, Materials[material]);
    }

    public Plane ParsePlane(InputStream inputFile, Scene scene)
    {
        ExpectSymbol(inputFile, "(");
        
        string material = ExpectIdentifier(inputFile);
        
        if (!Materials.ContainsKey(material))
            throw new SceneSyntaxException(inputFile.Location, $"unknown material {material}");

        ExpectSymbol(inputFile, ",");
        Transformation transformation = scene.ParseTransformation(inputFile);
        ExpectSymbol(inputFile, ")");

        return new Plane(transformation, Materials[material]);
    }

    public ICamera ParseCamera(InputStream inputFile, Scene scene)
    {
        ExpectSymbol(inputFile, "(");
        Keyword cameraKeyword = ExpectKeywords(inputFile, [Keyword.Perspective, Keyword.Orthogonal]);
        ExpectSymbol(inputFile, ",");
        Transformation transformation = scene.ParseTransformation(inputFile);
        ExpectSymbol(inputFile, ",");
        float aspectRatio = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ",");
        float distance = ExpectNumber(inputFile);
        ExpectSymbol(inputFile, ")");

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
                throw new SceneSyntaxException(inputFile.Location, "Keyword doesn't match any of the Cameras types");
        }

        return result;
    }

    public Scene ParseScene(InputStream inputFile, Dictionary<string, float> variables)
    {
        var scene = new Scene
        {
            Variables = variables
        };

        while (true)
        {
            Token token = inputFile.ReadNextToken();
            if (token is StopToken) break;
            if (token is not KeywordToken)
                throw new SceneSyntaxException(token.Location, $"expected keyword instead of {token}");
            if (token is KeywordToken { Keyword: Keyword.Float })
            {
                string variableName = ExpectIdentifier(inputFile);

                SourceLocation variableLocation = inputFile.Location;

                ExpectSymbol(inputFile, "(");
                float variableValue = ExpectNumber(inputFile);
                ExpectSymbol(inputFile, ")");

                if (scene.Variables.ContainsKey(variableName))
                    throw new SceneSyntaxException(variableLocation,
                        $"{variableName} cannot be redefined"); //Aggiungere controllo overridden variables 
                //Aggiungere controllo overridden variables anche qui
                scene.Variables[variableName] = variableValue;
            }
            else if (token is KeywordToken { Keyword: Keyword.Sphere })
            {
                var sphere = scene.ParseSphere(inputFile, scene);
                scene.World.Add(sphere);
            }
            else if (token is KeywordToken { Keyword: Keyword.Plane })
            {
                scene.World.Add(scene.ParsePlane(inputFile, scene));
            }
            else if (token is KeywordToken { Keyword: Keyword.Camera })
            {
                if (scene.Camera != null) throw new SceneSyntaxException(token.Location, "Cannot define more Cameras");
                
                scene.Camera = scene.ParseCamera(inputFile, scene);
            }
            else if (token is KeywordToken { Keyword: Keyword.Material })
            {
                scene.ParseMaterial(inputFile, out string name, out Material material);
                scene.Materials[name] = material;
            }
        }

        return scene;
    }
}