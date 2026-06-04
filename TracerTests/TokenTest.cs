using TracerLib;
using Xunit.Abstractions;

namespace TracerTests;

public class TokenTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private SourceLocation _location = new SourceLocation("", 1, 1);

    public TokenTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void KeywordTokenTest()
    {
       const string token = "orthogonal";

       Keywords.Map.TryGetValue(token, out var keyword);

       var keywordToken = new KeywordToken(_location, keyword);
       
       _testOutputHelper.WriteLine(keywordToken.ToString());
       
       Assert.Equal(1, keywordToken.Location.column);
       Assert.Equal(1, keywordToken.Location.line);
       Assert.Equal(keyword, keywordToken.Keyword);
    }
    
    [Fact]
    public void IdentifierTokenTest()
    {
        const string identifier = "clock";
        var identifierToken = new IdentifierToken(_location, identifier);
        
        _testOutputHelper.WriteLine(identifierToken.ToString());
        
        Assert.Equal(1, identifierToken.Location.column); 
        Assert.Equal(1, identifierToken.Location.line); 
        Assert.Equal("clock", identifierToken.Identifier);
    }

    [Fact]
    public void StringTokenTest()
    {
        const string s = "Hello, World!";

        var stringToken = new StringToken(_location, s);
        
        _testOutputHelper.WriteLine(stringToken.ToString());
        
         Assert.Equal(1, stringToken.Location.column);
         Assert.Equal(1, stringToken.Location.line);
         Assert.Equal("Hello, World!", stringToken.String);
    }

    [Fact]
    public void LiteralNumberToken()
    {
        const float f = 8.67f;

        var literalNumberToken = new LiteralNumberToken(_location, f);
        
        _testOutputHelper.WriteLine(literalNumberToken.ToString());
        
        Assert.Equal(1, literalNumberToken.Location.column);
        Assert.Equal(1, literalNumberToken.Location.line);
        Assert.Equal(8.67f, literalNumberToken.Value);
    }

    [Fact]
    public void SymbolTokenTest()
    {
        const string op = "+";

        var symbolToken = new SymbolToken(_location, op);
        
        _testOutputHelper.WriteLine(symbolToken.ToString());
        
        Assert.Equal(1, symbolToken.Location.column);
        Assert.Equal(1, symbolToken.Location.line);
        Assert.Equal("+", symbolToken.Symbol);
    }
}