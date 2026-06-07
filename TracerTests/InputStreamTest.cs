using TracerLib;
using Xunit.Abstractions;

namespace TracerTests;

public class InputStreamTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public InputStreamTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TestInputStreamConstructor()
    {
        string filePath = Path.GetTempFileName();
        try
        {
            SourceLocation location = new SourceLocation(filePath);
            InputStream str = new InputStream(filePath);

            Assert.Equal(location, str.Location);
            Assert.Equal(location, str.SavedLocation);
            Assert.Null(str.SavedChar);
            Assert.Equal(8, str.Tabulations);
            Assert.Null(str.SavedToken);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void TestUpdateLocation()
    {
        string filePath = Path.GetTempFileName();
        try
        {
            InputStream str = new InputStream(filePath);

            Assert.Equal(0, str.Location.line);
            Assert.Equal(0, str.Location.column);

            char ch = 'a';
            str.UpdateLocation(ch);
            Assert.Equal(0, str.Location.line);
            Assert.Equal(1, str.Location.column);

            ch = '\n';
            str.UpdateLocation(ch);
            Assert.Equal(1, str.Location.line);
            Assert.Equal(0, str.Location.column);

            ch = '\r';
            str.UpdateLocation(ch);
            Assert.Equal(2, str.Location.line);
            Assert.Equal(0, str.Location.column);

            ch = '\t';
            str.UpdateLocation(ch);
            Assert.Equal(2, str.Location.line);
            Assert.Equal(8, str.Location.column);

            ch = '\t';
            str.UpdateLocation(ch);
            Assert.Equal(2, str.Location.line);
            Assert.Equal(16, str.Location.column);

            ch = 'f';
            str.UpdateLocation(ch);
            Assert.Equal(2, str.Location.line);
            Assert.Equal(17, str.Location.column);

            ch = '\n';
            str.UpdateLocation(ch);
            Assert.Equal(3, str.Location.line);
            Assert.Equal(0, str.Location.column);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void TestReadChar_UnreadChar()
    {
        const string content = "abcde";
        string filePath = Path.GetTempFileName();
        try
        {
            File.WriteAllText(filePath, content);

            InputStream str = new InputStream(filePath);

            Assert.Equal('a', str.ReadChar());
            Assert.Equal(0, str.SavedLocation.line);
            Assert.Equal(0, str.SavedLocation.column);
            Assert.Equal(0, str.Location.line);
            Assert.Equal(1, str.Location.column);

            Assert.Equal('b', str.ReadChar());
            Assert.Equal(0, str.SavedLocation.line);
            Assert.Equal(1, str.SavedLocation.column);
            Assert.Equal(0, str.Location.line);
            Assert.Equal(2, str.Location.column);

            Assert.Equal('c', str.ReadChar());
            Assert.Equal(0, str.SavedLocation.line);
            Assert.Equal(2, str.SavedLocation.column);
            Assert.Equal(0, str.Location.line);
            Assert.Equal(3, str.Location.column);

            str.UnreadChar('c');
            Assert.Equal('c', str.SavedChar);
            Assert.Equal(0, str.SavedLocation.line);
            Assert.Equal(2, str.SavedLocation.column);
            Assert.Equal(0, str.Location.line);
            Assert.Equal(2, str.Location.column);

            Assert.Equal('c', str.ReadChar());
            Assert.Null(str.SavedChar);
            Assert.Equal(0, str.SavedLocation.line);
            Assert.Equal(2, str.SavedLocation.column);
            Assert.Equal(0, str.Location.line);
            Assert.Equal(3, str.Location.column);

            Assert.Equal('d', str.ReadChar());
            Assert.Equal(0, str.SavedLocation.line);
            Assert.Equal(3, str.SavedLocation.column);
            Assert.Equal(0, str.Location.line);
            Assert.Equal(4, str.Location.column);

            Assert.Equal('e', str.ReadChar());
            Assert.Equal(0, str.SavedLocation.line);
            Assert.Equal(4, str.SavedLocation.column);
            Assert.Equal(0, str.Location.line);
            Assert.Equal(5, str.Location.column);

            Assert.Null(str.ReadChar());
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void TestSkipLine()
    {
        const string content = "example dshhe, siunw 3,4,5\n" +
                               "pwsdjvr, g(ejsbd) \r wlsj \n" +
                               "kgiu, gfoian + odsn \r\n" +
                               "sodi, wkje.";
        string filePath = Path.GetTempFileName();
        try
        {
            File.WriteAllText(filePath, content);

            InputStream str = new InputStream(filePath);

            Assert.Equal('e', str.ReadChar());
            str.SkipLine();
            Assert.Equal('p', str.ReadChar());
            str.SkipLine();
            Assert.Equal('k', str.ReadChar());
            str.SkipLine();
            Assert.Equal('s', str.ReadChar());
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void TestSkipWhiteSpacesAndComments()
    {
        const string content = "d     sd\n" +
                               "\tr\n" +
                               "\t\r\n" +
                               "\n\n" +
                               "utn\t\r\t\n" +
                               "r";
        string filePath = Path.GetTempFileName();
        try
        {
            File.WriteAllText(filePath, content);

            InputStream str = new InputStream(filePath);

            Assert.Equal('d', str.ReadChar());
            str.SkipWhitespacesAndComments();
            Assert.Equal('s', str.ReadChar());
            str.ReadChar();
            str.SkipWhitespacesAndComments();
            Assert.Equal('r', str.ReadChar());
            str.SkipWhitespacesAndComments();
            Assert.Equal('u', str.ReadChar());

            str.SkipWhitespacesAndComments(); // Now it must not skip anything
            Assert.Equal('t', str.ReadChar());
            str.SkipWhitespacesAndComments(); // Now it must not skip anything
            Assert.Equal('n', str.ReadChar());

            str.SkipWhitespacesAndComments(); // Now it must not skip anything
            Assert.Equal('r', str.ReadChar());
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void TestParseStringToken()
    {
        const string content = "Hello, World!\"";
        const string fail = "Hello, World!";

        string filePath1 = Path.GetTempFileName();
        File.WriteAllText(filePath1, content);

        string filePath2 = Path.GetTempFileName();
        File.WriteAllText(filePath2, fail);

        try
        {
            var stream1 = new InputStream(filePath1);
            StringToken st1 = stream1._ParseStringToken(stream1.Location);
            Assert.Equal("Hello, World!", st1.String);

            var stream2 = new InputStream(filePath2);
            Assert.Throws<GrammarError>(() => stream2._ParseStringToken(stream2.Location));
        }
        finally
        {
            File.Delete(filePath1);
            File.Delete(filePath2);
        }
    }

    [Fact]
    public void TestParseFloatToken()
    {
        string content = "3496\n" +
                         "2.4\n" +
                         "6.92e3\n" +
                         "830003.1E9\n" +
                         "9.1e-4\n" +
                         "4.7E-12\n" +
                         "-34.3e5";
        string filePath1 = Path.GetTempFileName();
        File.WriteAllText(filePath1, content);
        try
        {
            var stream1 = new InputStream(filePath1);

            char? ch = stream1.ReadChar();
            LiteralNumberToken floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(3496, floatToken.Value);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(2.4f, floatToken.Value);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(6.92e3f, floatToken.Value);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(830003.1E9f, floatToken.Value);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(9.1e-4f, floatToken.Value);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(4.7E-12f, floatToken.Value);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(-34.3e5f, floatToken.Value);
        }
        finally
        {
            File.Delete(filePath1);
        }

        string fail = "a\n" +
                      "7..2\n" +
                      "4ee2\n" +
                      "--3.4\n" +
                      "3.5e--2";
        string filePath2 = Path.GetTempFileName();
        File.WriteAllText(filePath2, fail);
        try
        {
            var stream2 = new InputStream(filePath2);
            char? ch = stream2.ReadChar();
            Assert.Throws<GrammarError>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<GrammarError>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<GrammarError>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<GrammarError>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<GrammarError>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));
        }
        finally
        {
            File.Delete(filePath2);
        }
    }

    [Fact]
    public void TestParseKeywordToken()
    {
        string content = "new\n" +
                         "material\n" +
                         "plane\n" +
                         "sphere\n" +
                         "diffuse\n" +
                         "specular\n" +
                         "uniform\n" +
                         "checkered\n" +
                         "image\n" +
                         "rotation_x\n" +
                         "rotation_y\n" +
                         "rotation_z\n" +
                         "scaling\n" +
                         "camera\n" +
                         "orthogonal\n" +
                         "perspective\n" +
                         "float";
        string filePath1 = Path.GetTempFileName();
        File.WriteAllText(filePath1, content);
        try
        {
            var stream1 = new InputStream(filePath1);

            char? ch = stream1.ReadChar();
            KeywordToken keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.New, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Material, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Plane, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Sphere, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Diffuse, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Specular, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Uniform, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Checkered, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Image, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.RotationX, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.RotationY, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.RotationZ, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Scaling, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Camera, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Orthogonal, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Perspective, keywordToken.Keyword);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            keywordToken = (KeywordToken)stream1._ParseKeywordIdentifierToken(ch.Value, stream1.Location);
            Assert.Equal(Keyword.Float, keywordToken.Keyword);
        }
        finally
        {
            File.Delete(filePath1);
        }
    }

    [Fact]
    public void TestParseIdentifierToken()
    {
        string fail = "New\n" +
                      "nEw\n" +
                      "nnew\n" +
                      "n ew\n" +
                      "rotationX\n";
        string filePath2 = Path.GetTempFileName();
        File.WriteAllText(filePath2, fail);
        try
        {
            var stream2 = new InputStream(filePath2);
            char? ch = stream2.ReadChar();
            Assert.Throws<GrammarError>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<GrammarError>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            
        }
        finally
        {
            File.Delete(filePath2);
        }
    }
    //TEST readToken

    [Fact]
    public void TestSceneFile()
    {
        const string content = "abc   \nd\nef";

        string filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, content);

        try
        {
            var stream = new InputStream(filePath);

            Assert.Equal(0, stream.Location.column);
            Assert.Equal(0, stream.Location.line);

            Assert.Equal('a', stream.ReadChar());
            Assert.Equal(1, stream.Location.column);
            Assert.Equal(0, stream.Location.line);

            stream.UnreadChar('a');
            Assert.Equal(0, stream.Location.column);
            Assert.Equal(0, stream.Location.line);

            Assert.Equal('a', stream.ReadChar());
            Assert.Equal(1, stream.Location.column);
            Assert.Equal(0, stream.Location.line);

            Assert.Equal('b', stream.ReadChar());
            Assert.Equal(2, stream.Location.column);
            Assert.Equal(0, stream.Location.line);

            Assert.Equal('c', stream.ReadChar());
            Assert.Equal(3, stream.Location.column);
            Assert.Equal(0, stream.Location.line);

            stream.SkipWhitespacesAndComments();

            Assert.Equal(0, stream.Location.column);
            Assert.Equal(1, stream.Location.line);

            Assert.Equal('d', stream.ReadChar());
            Assert.Equal(1, stream.Location.column);
            Assert.Equal(1, stream.Location.line);

            Assert.Equal('\n', stream.ReadChar());
            Assert.Equal(0, stream.Location.column);
            Assert.Equal(2, stream.Location.line);

            Assert.Equal('e', stream.ReadChar());
            Assert.Equal(1, stream.Location.column);
            Assert.Equal(2, stream.Location.line);

            Assert.Equal('f', stream.ReadChar());
            Assert.Equal(2, stream.Location.column);
            Assert.Equal(2, stream.Location.line);

            Assert.Null(stream.ReadChar());
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}