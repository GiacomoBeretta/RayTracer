namespace TracerLib;

//Verificare se modificare {token} in {typetoken} nei messaggi d'errore cambia qualcosa
//Considerare di cambiare ExpecteSymbol(InputStream, string) in ExpecteSymbol(InputStream, char)
public class Scene
{
    public Dictionary<string, Material> Materials { get; set; }
    public World World { get; set; }
    public ICamera? Camera { get; set; }
    public Dictionary<string, float> Variables { get; set; }
    //Overridden variables???

    public Scene()
    {
        Materials = new Dictionary<string, Material>();
        World = new World();
        Camera = null;
        Variables = new Dictionary<string, float>();
    }

    public void ExpectSymbol(InputStream inputFile, string symbol)
    {
        var token = inputFile.ReadNextToken();
        if (token is not SymbolToken symbolToken || symbolToken.Symbol != symbol)
        {
            throw new GrammarError(token.Location, $"got {token} instead of {symbol}");
        }
    }

    public Keyword ExpectKeywords(InputStream inputFile, List<Keyword> keywords)
    {
        var token = inputFile.ReadNextToken();

        if (token is not KeywordToken keywordToken)
        {
            throw new GrammarError(token.Location, $"expected keyword instead of {token}");
        } else if (!keywords.Contains(keywordToken.Keyword))
        {
            throw new GrammarError(token.Location,
                $"expect one of the following keywords: {string.Join(',', keywords)} instead of {token}");
        }

        return keywordToken.Keyword;
    }

    public float ExpectNumber(InputStream inputFile, Scene scene)
    {
        var token = inputFile.ReadNextToken();

        if (token is LiteralNumberToken literalNumberToken) return literalNumberToken.Value;
        else if (token is IdentifierToken identifierToken)
        {
            var variableName = identifierToken.Identifier;
            if (!scene.Variables.ContainsKey(variableName))
                throw new GrammarError(token.Location, $"unknow variable {token}");
            return scene.Variables[variableName];
        }

        throw new GrammarError(token.Location, $"got {token} instead of a number");
    }

    public string ExpectString(InputStream inputFile)
    {
        var token = inputFile.ReadNextToken();

        if (token is not StringToken stringToken)
            throw new GrammarError(token.Location, $"got {token} instead of a string");

        return stringToken.String;
    }

    public string ExpectIdentifier(InputStream inputFile)
    {
        var token = inputFile.ReadNextToken();

        if (token is not IdentifierToken identifierToken) throw new GrammarError(token.Location, $"got {token} instead of an identifier");

        return identifierToken.Identifier;
    }

    public Vector ParseVector(InputStream inputFile, Scene scene)
    {
        ExpectSymbol(inputFile, "[");
        var x = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, ",");
        var y = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, ",");
        var z = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, "]");

        return new Vector(x, y, z);
    }
    
    public Color ParseColor(InputStream inputFile, Scene scene)
    {
        ExpectSymbol(inputFile, "<");
        var r = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, ",");
        var g = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, ",");
        var b = ExpectNumber(inputFile, scene);
        ExpectSymbol(inputFile, ">");

        return new Color(r, g, b);
    }

    public Pigment ParsePigment(InputStream inputFile, Scene scene)
    {
        var keyword = ExpectKeywords(inputFile, [Keyword.Uniform, Keyword.Checkered, Keyword.Image]);
        Pigment result;
        
        ExpectSymbol(inputFile, "(");

        switch (keyword)
        {
            case Keyword.Uniform:
                var color = ParseColor(inputFile, scene);
                result = new UniformPigment(color);
                break;
            case Keyword.Checkered:
                var color1 = ParseColor(inputFile, scene);
                ExpectSymbol(inputFile, ",");
                var color2 = ParseColor(inputFile, scene);
                ExpectSymbol(inputFile, ",");
                var steps = (int)ExpectNumber(inputFile, scene);
                result = new CheckeredPigment(color1, color2, steps);
                break;
            case Keyword.Image:
                var fileName = ExpectString(inputFile);
                using (FileStream imageFile = File.OpenRead(fileName))
                {
                    var image = HDRImage.ReadPFM_File(imageFile);
                    result = new ImagePigment(image);
                }
                break;
                default:
                throw new GrammarError("Keyword doesn't match any of the Pigments types");
        }
        
        ExpectSymbol(inputFile, ")");
        return result;
    }

    public BRDF ParseBRDF(InputStream inputFile, Scene scene)
    {
        var BRDFkeyword = ExpectKeywords(inputFile, [Keyword.Diffuse, Keyword.Specular]);
        ExpectSymbol(inputFile, "(");
        var pigment = ParsePigment(inputFile, scene);
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
                throw new GrammarError("Keyword doesn't match any of the BRDF types");
        }

        return result;
    }

    public void ParseMaterial(InputStream inputFile, Scene scene, out string name, out Material material)
    {
        name = ExpectIdentifier(inputFile);
        
        ExpectSymbol(inputFile, "(");
        var brdf = ParseBRDF(inputFile, scene);
        ExpectSymbol(inputFile, ",");
        var emittedRadiance = ParsePigment(inputFile, scene);
        ExpectSymbol(inputFile, ")");

        material = new Material(emittedRadiance, brdf);
    }
}