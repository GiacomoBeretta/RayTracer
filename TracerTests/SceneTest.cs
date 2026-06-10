using System.Net;
using TracerLib;

namespace TracerTests;

public class SceneTest
{
    private Scene scene = new Scene();

    [Fact]
    public void ExpectSymbolTest()
    {
        const string symbol = "(),[]<>*";
        
        string filepath = Path.GetTempFileName();
        File.WriteAllText(filepath, symbol);
        
        try
        {
            var str = new InputStream(filepath);
            
            scene.ExpectSymbol(str, "(");
            scene.ExpectSymbol(str, ")");
            Assert.Throws<GrammarError>(() => scene.ExpectSymbol(str, "Pizza"));
            scene.ExpectSymbol(str, "[");
            scene.ExpectSymbol(str, "]");
            scene.ExpectSymbol(str, "<");
            scene.ExpectSymbol(str, ">");
            scene.ExpectSymbol(str, "*");

        }
        finally
        {
            File.Delete(filepath);
        }
    }

    [Fact]
    public void ExpectKeywordTest()
    {
        const string key = "identity  scaling  saling";
        List<Keyword> keywords = [Keyword.RotationX, Keyword.RotationY, Keyword.Scaling];

        string filepath = Path.GetTempFileName();
        File.WriteAllText(filepath, key);

        try
        {
            var str = new InputStream(filepath);

            Assert.Throws<GrammarError>(() => scene.ExpectKeywords(str, keywords));
            Assert.Equal(Keyword.Scaling, scene.ExpectKeywords(str, keywords));
            Assert.Throws<GrammarError>(() => scene.ExpectKeywords(str, keywords));
        }
        finally
        {
            File.Delete(filepath);
        }
    }

    [Fact]
    public void ExpectNumberTest()
    {
        const string number = "( 18+15  1e18";
        
        string filepath = Path.GetTempFileName();
        File.WriteAllText(filepath, number);

        try
        {
            var str = new InputStream(filepath);
            Assert.Throws<GrammarError>(() => scene.ExpectNumber(str, scene));
            Assert.Equal(18f, scene.ExpectNumber(str, scene));
            //Controllare perchè il segno + viene saltato
            Assert.Equal(15f, scene.ExpectNumber(str, scene));
            Assert.True(Functions.AreClose(1e18f, scene.ExpectNumber(str, scene)));
        }
        finally
        {
            File.Delete(filepath);
        }
    }

    [Fact]
    public void ExpectStringToken()
    {
        const string content = "6  identity  \"pizza\"";
        
        string filepath = Path.GetTempFileName();
        File.WriteAllText(filepath, content);

        try
        {
            var str = new InputStream(filepath);

            Assert.Throws<GrammarError>(() => scene.ExpectString(str));
            Assert.Throws<GrammarError>(() => scene.ExpectString(str));
            Assert.Equal("pizza", scene.ExpectString(str));
        }
        finally
        {
            File.Delete(filepath);
        }
    }

    [Fact]
    public void ExpectIdentifier()
    {
        const string content = "clock  identity   \"Hello world\"";

        string filepath = Path.GetTempFileName();
        File.WriteAllText(filepath, content);

        try
        {
            var str = new InputStream(filepath);
            
            Assert.Equal("clock", scene.ExpectIdentifier(str));
            Assert.Throws<GrammarError>(() => scene.ExpectIdentifier(str));
            Assert.Throws<GrammarError>(() => scene.ExpectIdentifier(str));
        }
        finally
        {
            File.Delete(filepath);
        }
    }
}