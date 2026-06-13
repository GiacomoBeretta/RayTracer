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
        using (InputStream str = new InputStream(filePath))
        {
            SourceLocation location = new SourceLocation(filePath);

            Assert.Equal(location, str.Location);
            Assert.Equal(location, str.SavedLocation);
            Assert.Null(str.SavedChar);
            Assert.Equal(8, str.Tabulations);
            Assert.Null(str.SavedToken);
        }

        File.Delete(filePath);
        /* try
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
         }*/
    }

    [Fact]
    public void TestUpdateLocation()
    {
        string filePath = Path.GetTempFileName();
        using (InputStream str = new InputStream(filePath))
        {
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

        File.Delete(filePath);
    }

    [Fact]
    public void TestReadChar_UnreadChar()
    {
        const string content = "abcde";
        string filePath = Path.GetTempFileName();

        File.WriteAllText(filePath, content);

        using (InputStream str = new InputStream(filePath))
        {
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

        File.Delete(filePath);
    }

    [Fact]
    public void TestSkipLine()
    {
        const string content = "example dshhe, siunw 3,4,5\n" +
                               "pwsdjvr, g(ejsbd) \r wlsj \n" +
                               "kgiu, gfoian + odsn \r\n" +
                               "sodi, wkje.";
        string filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, content);
        using (InputStream str = new InputStream(filePath))
        {
            Assert.Equal('e', str.ReadChar());
            str.SkipLine();
            Assert.Equal('p', str.ReadChar());
            str.SkipLine();
            Assert.Equal('k', str.ReadChar());
            str.SkipLine();
            Assert.Equal('s', str.ReadChar());

            //if you try to skip the last line without any escape sequence then is reached the end of file
            str.SkipLine();
            Assert.Null(str.ReadChar());
        }

        File.Delete(filePath);
    }

    [Fact]
    public void TestSkipWhiteSpacesAndComments()
    {
        const string content = "d     sd\n" +
                               "\tr\n" +
                               "\t\r\n" +
                               "\n\n" +
                               "utn\t\r\t\n" +
                               "r    ";
        string filePath = Path.GetTempFileName();

        File.WriteAllText(filePath, content);

        using (InputStream str = new InputStream(filePath))
        {
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

            //assert that the end of file is reached
            str.SkipWhitespacesAndComments();
            Assert.Null(str.ReadChar());
        }

        File.Delete(filePath);
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

        using (InputStream stream1 = new InputStream(filePath1))
        {
            StringToken st1 = stream1._ParseStringToken(stream1.Location);
            Assert.Equal("Hello, World!", st1.String);
        }

        using (InputStream stream2 = new InputStream(filePath2))
        {
            Assert.Throws<SceneSyntaxException>(() => stream2._ParseStringToken(stream2.Location));
        }

        File.Delete(filePath1);
        File.Delete(filePath2);
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
                         "-34.3e5\n" +
                         "+6.5\n" +
                         "-8\n" +
                         "9.5e+5\n" +
                         "2.3E-6\n" +
                         "+4e+7\n" +
                         "-8.9e-3\n" +
                         "3+4\n" +
                         "5.2e+10-3";
        string filePath1 = Path.GetTempFileName();
        File.WriteAllText(filePath1, content);
        using (InputStream stream1 = new InputStream(filePath1))
        {
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

            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(6.5f, floatToken.Value);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(-8f, floatToken.Value);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(9.5e+5f, floatToken.Value);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(2.3E-6f, floatToken.Value);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(4e+7f, floatToken.Value);

            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(-8.9e-3f, floatToken.Value);

            // 3+4
            stream1.SkipLine();
            ch = stream1.ReadChar(); // '3'
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(3f, floatToken.Value);
            ch = stream1.ReadChar(); // '+'
            ch = stream1.ReadChar(); // '4'
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(4f, floatToken.Value);

            // 5.2e+10-3
            stream1.SkipLine();
            ch = stream1.ReadChar();
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location); // '5.2e+10'
            Assert.Equal(5.2e10f, floatToken.Value);
            ch = stream1.ReadChar(); // '-'
            ch = stream1.ReadChar(); // '3'
            floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
            Assert.Equal(3f, floatToken.Value);
        }

        File.Delete(filePath1);

        string fail = "a\n" +
                      "++3\n" +
                      "--6.8\n" +
                      "+.3\n" +
                      "-e4\n" +
                      "4.\n" +
                      "8.e6\n" +
                      "7..2\n" +
                      "4ee2\n" +
                      "3.5e--2";
        string filePath2 = Path.GetTempFileName();
        File.WriteAllText(filePath2, fail);
        using (InputStream stream2 = new InputStream(filePath2))
        {
            // a
            char? ch = stream2.ReadChar();
            Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            // ++3
            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            // --6.8
            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            // +.3
            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            // -e4
            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            // 4.
            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            // 8.e6
            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            // 7..2
            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            // 4ee2
            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

            // 3.5e--2
            stream2.SkipLine();
            ch = stream2.ReadChar();
            Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));
        }

        File.Delete(filePath2);
    }

    [Fact]
    public void TestParseKeywordIdentifierToken()
    {
        // keyword parse
        string content1 = "new\n" +
                          "material\n" +
                          "plane\n" +
                          "sphere\n" +
                          "diffuse\n" +
                          "specular\n" +
                          "uniform\n" +
                          "checkered\n" +
                          "image\n" +
                          "identity\n" +
                          "translation\n" +
                          "rotation_x\n" +
                          "rotation_y\n" +
                          "rotation_z\n" +
                          "scaling\n" +
                          "camera\n" +
                          "orthogonal\n" +
                          "perspective\n" +
                          "float";
        string filePath1 = Path.GetTempFileName();
        File.WriteAllText(filePath1, content1);
        using (InputStream str = new InputStream(filePath1))
        {
            char? ch = str.ReadChar();
            KeywordToken keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.New, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Material, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Plane, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Sphere, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Diffuse, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Specular, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Uniform, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Checkered, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Image, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Identity, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Translation, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.RotationX, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.RotationY, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.RotationZ, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Scaling, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Camera, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Orthogonal, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Perspective, keywordToken.Keyword);

            str.SkipLine();
            ch = str.ReadChar();
            keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal(Keyword.Float, keywordToken.Keyword);
        }

        File.Delete(filePath1);


        // identifier parse
        string content2 = "a\n" +
                          "_tmp\n" +
                          "variable\n" +
                          "r3\n" +
                          "t_2_float_\n" +
                          "___4a\n" +
                          "New\n" +
                          "nEw\n" +
                          "nnew\n" +
                          "rotationX\n" +
                          "a b"; // fail
        string filePath2 = Path.GetTempFileName();
        File.WriteAllText(filePath2, content2);
        using (InputStream str = new InputStream(filePath2))
        {
            char? ch = str.ReadChar();
            IdentifierToken identifierToken =
                (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal("a", identifierToken.Identifier);

            str.SkipLine();
            ch = str.ReadChar();
            identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal("_tmp", identifierToken.Identifier);

            str.SkipLine();
            ch = str.ReadChar();
            identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal("variable", identifierToken.Identifier);

            str.SkipLine();
            ch = str.ReadChar();
            identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal("r3", identifierToken.Identifier);

            str.SkipLine();
            ch = str.ReadChar();
            identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal("t_2_float_", identifierToken.Identifier);

            str.SkipLine();
            ch = str.ReadChar();
            identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal("___4a", identifierToken.Identifier);

            str.SkipLine();
            ch = str.ReadChar();
            identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal("New", identifierToken.Identifier);

            str.SkipLine();
            ch = str.ReadChar();
            identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal("nEw", identifierToken.Identifier);

            str.SkipLine();
            ch = str.ReadChar();
            identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal("nnew", identifierToken.Identifier);

            str.SkipLine();
            ch = str.ReadChar();
            identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.Equal("rotationX", identifierToken.Identifier);

            str.SkipLine();
            ch = str.ReadChar();
            identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
            Assert.NotEqual("a b", identifierToken.Identifier);
        }

        File.Delete(filePath2);
    }

    [Fact]
    public void TestReadNextToken()
    {
        const string content = """
                               # Declare a floating-point variable named "clock"
                               float clock(150)

                               # Declare a few new materials. Each of them includes a BRDF and a pigment

                               # We can split a definition over multiple lines and indent them as we like
                               material sky_material(
                                   diffuse(image("sky-dome.pfm")),
                                   uniform(<0.7, 0.5, 1>)
                               )

                               material ground_material(
                                   diffuse(checkered(<0.3, 0.5, 0.1>,
                                                     <0.1, 0.2, 0.5>, 4)),
                                   uniform(<0, 0, 0>)
                               )

                               material sphere_material(
                                   specular(uniform(<0.5, 0.5, 0.5>)),
                                   uniform(<0, 0, 0>)
                               )

                               # Define a few shapes
                               sphere(sphere_material, translation([0, 0, 1]))

                               # The language is flexible enough to permit spaces before "("
                               plane (ground_material, identity)

                               # Here we use the "clock" variable! Note that vectors are notated using
                               # square brackets ([]) instead of angular brackets (<>) like colors, and
                               # that we can compose transformations through the "*" operator
                               plane(sky_material, translation([0, 0, 100]) * rotation_y(clock))

                               # Define a camera
                               camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 1.0)
                               """;
        string filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, content);

        using (InputStream stream = new InputStream(filePath))
        {
            KeywordToken keywordToken;
            IdentifierToken identifierToken;
            StringToken stringToken;
            LiteralNumberToken numberToken;
            SymbolToken symbolToken;

            // line 1: float clock(150) - begin

            #region line 1

            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Float, keywordToken.Keyword);
            identifierToken = (IdentifierToken)stream.ReadNextToken();
            Assert.Equal("clock", identifierToken.Identifier);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(150f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);

            #endregion

            // line 2: material sky_material(diffuse(image("sky-dome.pfm")),uniform(<0.7, 0.5, 1>))

            #region line 2

            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Material, keywordToken.Keyword);
            identifierToken = (IdentifierToken)stream.ReadNextToken();
            Assert.Equal("sky_material", identifierToken.Identifier);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Diffuse, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Image, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            stringToken = (StringToken)stream.ReadNextToken();
            Assert.Equal("sky-dome.pfm", stringToken.String);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Uniform, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("<", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0.7f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0.5f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(1f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(">", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);

            #endregion

            // line 3: material ground_material(diffuse(checkered(<0.3, 0.5, 0.1>,<0.1, 0.2, 0.5>, 4)),uniform(<0, 0, 0>))

            #region line 3

            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Material, keywordToken.Keyword);
            identifierToken = (IdentifierToken)stream.ReadNextToken();
            Assert.Equal("ground_material", identifierToken.Identifier);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Diffuse, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Checkered, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("<", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0.3f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0.5f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0.1f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(">", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("<", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0.1f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0.2f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0.5f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(">", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(4f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Uniform, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("<", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(">", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);

            #endregion

            // line 4: material sphere_material(specular(uniform(<0.5, 0.5, 0.5>)),uniform(<0, 0, 0>))

            #region line 4

            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Material, keywordToken.Keyword);
            identifierToken = (IdentifierToken)stream.ReadNextToken();
            Assert.Equal("sphere_material", identifierToken.Identifier);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Specular, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Uniform, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("<", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0.5f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0.5f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0.5f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(">", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Uniform, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("<", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(">", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);

            #endregion

            // line 5: sphere(sphere_material, translation([0, 0, 1]))

            #region line 5

            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Sphere, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            identifierToken = (IdentifierToken)stream.ReadNextToken();
            Assert.Equal("sphere_material", identifierToken.Identifier);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Translation, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("[", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(1f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("]", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);

            #endregion

            // line 6: plane (ground_material, identity)

            #region line 6

            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Plane, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            identifierToken = (IdentifierToken)stream.ReadNextToken();
            Assert.Equal("ground_material", identifierToken.Identifier);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Identity, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);

            #endregion

            // line 7: plane(sky_material, translation([0, 0, 100]) * rotation_y(clock))

            #region line 7

            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Plane, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            identifierToken = (IdentifierToken)stream.ReadNextToken();
            Assert.Equal("sky_material", identifierToken.Identifier);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Translation, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("[", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(100f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("]", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("*", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.RotationY, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            identifierToken = (IdentifierToken)stream.ReadNextToken();
            Assert.Equal("clock", identifierToken.Identifier);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);

            #endregion

            // line 8: camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 1.0)

            #region line 8

            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Camera, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Perspective, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.RotationZ, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(30f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("*", symbolToken.Symbol);
            keywordToken = (KeywordToken)stream.ReadNextToken();
            Assert.Equal(Keyword.Translation, keywordToken.Keyword);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("(", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("[", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(-4f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(1f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal("]", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(1.0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(",", symbolToken.Symbol);
            numberToken = (LiteralNumberToken)stream.ReadNextToken();
            Assert.Equal(1.0f, numberToken.Value);
            symbolToken = (SymbolToken)stream.ReadNextToken();
            Assert.Equal(")", symbolToken.Symbol);

            #endregion

            Assert.Equal(typeof(StopToken), ((StopToken)stream.ReadNextToken()).GetType());
        }

        File.Delete(filePath);
    }

    [Fact]
    public void TestSceneFile()
    {
        const string content = "abc   \nd\nef";

        string filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, content);

        using (InputStream stream = new InputStream(filePath))
        {
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

        File.Delete(filePath);
    }

/*   [Fact]
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

           //if you try to skip the last line without any escape sequence then is reached the end of file
           str.SkipLine();
           Assert.Null(str.ReadChar());
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
                              "r    ";
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

           //assert that the end of file is reached
           str.SkipWhitespacesAndComments();
           Assert.Null(str.ReadChar());
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
           InputStream stream1 = new InputStream(filePath1);
           StringToken st1 = stream1._ParseStringToken(stream1.Location);
           Assert.Equal("Hello, World!", st1.String);

           InputStream stream2 = new InputStream(filePath2);
           Assert.Throws<SceneSyntaxException>(() => stream2._ParseStringToken(stream2.Location));
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
                        "-34.3e5\n" +
                        "+6.5\n" +
                        "-8\n" +
                        "9.5e+5\n" +
                        "2.3E-6\n" +
                        "+4e+7\n" +
                        "-8.9e-3\n" +
                        "3+4\n" +
                        "5.2e+10-3";
       string filePath1 = Path.GetTempFileName();
       File.WriteAllText(filePath1, content);
       try
       {
           InputStream stream1 = new InputStream(filePath1);

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

           stream1.SkipLine();
           ch = stream1.ReadChar();
           floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
           Assert.Equal(6.5f, floatToken.Value);

           stream1.SkipLine();
           ch = stream1.ReadChar();
           floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
           Assert.Equal(-8f, floatToken.Value);

           stream1.SkipLine();
           ch = stream1.ReadChar();
           floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
           Assert.Equal(9.5e+5f, floatToken.Value);

           stream1.SkipLine();
           ch = stream1.ReadChar();
           floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
           Assert.Equal(2.3E-6f, floatToken.Value);

           stream1.SkipLine();
           ch = stream1.ReadChar();
           floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
           Assert.Equal(4e+7f, floatToken.Value);

           stream1.SkipLine();
           ch = stream1.ReadChar();
           floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
           Assert.Equal(-8.9e-3f, floatToken.Value);

           // 3+4
           stream1.SkipLine();
           ch = stream1.ReadChar(); // '3'
           floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
           Assert.Equal(3f, floatToken.Value);
           ch = stream1.ReadChar(); // '+'
           ch = stream1.ReadChar(); // '4'
           floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
           Assert.Equal(4f, floatToken.Value);

           // 5.2e+10-3
           stream1.SkipLine();
           ch = stream1.ReadChar();
           floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location); // '5.2e+10'
           Assert.Equal(5.2e10f, floatToken.Value);
           ch = stream1.ReadChar(); // '-'
           ch = stream1.ReadChar(); // '3'
           floatToken = stream1._ParseFloatToken(ch.Value, stream1.Location);
           Assert.Equal(3f, floatToken.Value);
       }
       finally
       {
           File.Delete(filePath1);
       }

       string fail = "a\n" +
                     "++3\n" +
                     "--6.8\n" +
                     "+.3\n" +
                     "-e4\n" +
                     "4.\n" +
                     "8.e6\n" +
                     "7..2\n" +
                     "4ee2\n" +
                     "3.5e--2";
       string filePath2 = Path.GetTempFileName();
       File.WriteAllText(filePath2, fail);
       try
       {
           // a
           InputStream stream2 = new InputStream(filePath2);
           char? ch = stream2.ReadChar();
           Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

           // ++3
           stream2.SkipLine();
           ch = stream2.ReadChar();
           Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

           // --6.8
           stream2.SkipLine();
           ch = stream2.ReadChar();
           Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

           // +.3
           stream2.SkipLine();
           ch = stream2.ReadChar();
           Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

           // -e4
           stream2.SkipLine();
           ch = stream2.ReadChar();
           Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

           // 4.
           stream2.SkipLine();
           ch = stream2.ReadChar();
           Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

           // 8.e6
           stream2.SkipLine();
           ch = stream2.ReadChar();
           Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

           // 7..2
           stream2.SkipLine();
           ch = stream2.ReadChar();
           Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

           // 4ee2
           stream2.SkipLine();
           ch = stream2.ReadChar();
           Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));

           // 3.5e--2
           stream2.SkipLine();
           ch = stream2.ReadChar();
           Assert.Throws<SceneSyntaxException>(() => stream2._ParseFloatToken(ch.Value, stream2.Location));
       }
       finally
       {
           File.Delete(filePath2);
       }
   }

   [Fact]
   public void TestParseKeywordIdentifierToken()
   {
       // keyword parse
       string content1 = "new\n" +
                         "material\n" +
                         "plane\n" +
                         "sphere\n" +
                         "diffuse\n" +
                         "specular\n" +
                         "uniform\n" +
                         "checkered\n" +
                         "image\n" +
                         "identity\n" +
                         "translation\n" +
                         "rotation_x\n" +
                         "rotation_y\n" +
                         "rotation_z\n" +
                         "scaling\n" +
                         "camera\n" +
                         "orthogonal\n" +
                         "perspective\n" +
                         "float";
       string filePath1 = Path.GetTempFileName();
       File.WriteAllText(filePath1, content1);
       try
       {
           InputStream str = new InputStream(filePath1);

           char? ch = str.ReadChar();
           KeywordToken keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.New, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Material, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Plane, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Sphere, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Diffuse, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Specular, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Uniform, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Checkered, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Image, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Identity, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Translation, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.RotationX, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.RotationY, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.RotationZ, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Scaling, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Camera, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Orthogonal, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Perspective, keywordToken.Keyword);

           str.SkipLine();
           ch = str.ReadChar();
           keywordToken = (KeywordToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal(Keyword.Float, keywordToken.Keyword);
       }
       finally
       {
           File.Delete(filePath1);
       }

       // identifier parse
       string content2 = "a\n" +
                         "_tmp\n" +
                         "variable\n" +
                         "r3\n" +
                         "t_2_float_\n" +
                         "___4a\n" +
                         "New\n" +
                         "nEw\n" +
                         "nnew\n" +
                         "rotationX\n" +
                         "a b"; // fail
       string filePath2 = Path.GetTempFileName();
       File.WriteAllText(filePath2, content2);
       try
       {
           InputStream str = new InputStream(filePath2);
           char? ch = str.ReadChar();
           IdentifierToken identifierToken =
               (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal("a", identifierToken.Identifier);

           str.SkipLine();
           ch = str.ReadChar();
           identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal("_tmp", identifierToken.Identifier);

           str.SkipLine();
           ch = str.ReadChar();
           identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal("variable", identifierToken.Identifier);

           str.SkipLine();
           ch = str.ReadChar();
           identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal("r3", identifierToken.Identifier);

           str.SkipLine();
           ch = str.ReadChar();
           identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal("t_2_float_", identifierToken.Identifier);

           str.SkipLine();
           ch = str.ReadChar();
           identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal("___4a", identifierToken.Identifier);

           str.SkipLine();
           ch = str.ReadChar();
           identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal("New", identifierToken.Identifier);

           str.SkipLine();
           ch = str.ReadChar();
           identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal("nEw", identifierToken.Identifier);

           str.SkipLine();
           ch = str.ReadChar();
           identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal("nnew", identifierToken.Identifier);

           str.SkipLine();
           ch = str.ReadChar();
           identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.Equal("rotationX", identifierToken.Identifier);

           str.SkipLine();
           ch = str.ReadChar();
           identifierToken = (IdentifierToken)str._ParseKeywordIdentifierToken(ch.Value, str.Location);
           Assert.NotEqual("a b", identifierToken.Identifier);
       }
       finally
       {
           File.Delete(filePath2);
       }
   }

   [Fact]
   public void TestReadNextToken()
   {
       const string content = """
                              # Declare a floating-point variable named "clock"
                              float clock(150)

                              # Declare a few new materials. Each of them includes a BRDF and a pigment

                              # We can split a definition over multiple lines and indent them as we like
                              material sky_material(
                                  diffuse(image("sky-dome.pfm")),
                                  uniform(<0.7, 0.5, 1>)
                              )

                              material ground_material(
                                  diffuse(checkered(<0.3, 0.5, 0.1>,
                                                    <0.1, 0.2, 0.5>, 4)),
                                  uniform(<0, 0, 0>)
                              )

                              material sphere_material(
                                  specular(uniform(<0.5, 0.5, 0.5>)),
                                  uniform(<0, 0, 0>)
                              )

                              # Define a few shapes
                              sphere(sphere_material, translation([0, 0, 1]))

                              # The language is flexible enough to permit spaces before "("
                              plane (ground_material, identity)

                              # Here we use the "clock" variable! Note that vectors are notated using
                              # square brackets ([]) instead of angular brackets (<>) like colors, and
                              # that we can compose transformations through the "*" operator
                              plane(sky_material, translation([0, 0, 100]) * rotation_y(clock))

                              # Define a camera
                              camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 1.0)
                              """;
       string filePath = Path.GetTempFileName();
       File.WriteAllText(filePath, content);

       try
       {
           InputStream stream = new InputStream(filePath);
           KeywordToken keywordToken;
           IdentifierToken identifierToken;
           StringToken stringToken;
           LiteralNumberToken numberToken;
           SymbolToken symbolToken;

           // line 1: float clock(150) - begin

           #region line 1

           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Float, keywordToken.Keyword);
           identifierToken = (IdentifierToken)stream.ReadNextToken();
           Assert.Equal("clock", identifierToken.Identifier);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(150f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);

           #endregion

           // line 2: material sky_material(diffuse(image("sky-dome.pfm")),uniform(<0.7, 0.5, 1>))

           #region line 2

           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Material, keywordToken.Keyword);
           identifierToken = (IdentifierToken)stream.ReadNextToken();
           Assert.Equal("sky_material", identifierToken.Identifier);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Diffuse, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Image, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           stringToken = (StringToken)stream.ReadNextToken();
           Assert.Equal("sky-dome.pfm", stringToken.String);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Uniform, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("<", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0.7f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0.5f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(1f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(">", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);

           #endregion

           // line 3: material ground_material(diffuse(checkered(<0.3, 0.5, 0.1>,<0.1, 0.2, 0.5>, 4)),uniform(<0, 0, 0>))

           #region line 3

           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Material, keywordToken.Keyword);
           identifierToken = (IdentifierToken)stream.ReadNextToken();
           Assert.Equal("ground_material", identifierToken.Identifier);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Diffuse, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Checkered, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("<", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0.3f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0.5f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0.1f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(">", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("<", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0.1f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0.2f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0.5f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(">", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(4f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Uniform, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("<", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(">", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);

           #endregion

           // line 4: material sphere_material(specular(uniform(<0.5, 0.5, 0.5>)),uniform(<0, 0, 0>))

           #region line 4

           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Material, keywordToken.Keyword);
           identifierToken = (IdentifierToken)stream.ReadNextToken();
           Assert.Equal("sphere_material", identifierToken.Identifier);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Specular, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Uniform, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("<", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0.5f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0.5f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0.5f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(">", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Uniform, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("<", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(">", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);

           #endregion

           // line 5: sphere(sphere_material, translation([0, 0, 1]))

           #region line 5

           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Sphere, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           identifierToken = (IdentifierToken)stream.ReadNextToken();
           Assert.Equal("sphere_material", identifierToken.Identifier);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Translation, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("[", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(1f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("]", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);

           #endregion

           // line 6: plane (ground_material, identity)

           #region line 6

           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Plane, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           identifierToken = (IdentifierToken)stream.ReadNextToken();
           Assert.Equal("ground_material", identifierToken.Identifier);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Identity, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);

           #endregion

           // line 7: plane(sky_material, translation([0, 0, 100]) * rotation_y(clock))

           #region line 7

           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Plane, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           identifierToken = (IdentifierToken)stream.ReadNextToken();
           Assert.Equal("sky_material", identifierToken.Identifier);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Translation, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("[", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(100f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("]", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("*", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.RotationY, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           identifierToken = (IdentifierToken)stream.ReadNextToken();
           Assert.Equal("clock", identifierToken.Identifier);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);

           #endregion

           // line 8: camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 1.0)

           #region line 8

           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Camera, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Perspective, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.RotationZ, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(30f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("*", symbolToken.Symbol);
           keywordToken = (KeywordToken)stream.ReadNextToken();
           Assert.Equal(Keyword.Translation, keywordToken.Keyword);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("(", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("[", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(-4f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(1f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal("]", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(1.0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(",", symbolToken.Symbol);
           numberToken = (LiteralNumberToken)stream.ReadNextToken();
           Assert.Equal(1.0f, numberToken.Value);
           symbolToken = (SymbolToken)stream.ReadNextToken();
           Assert.Equal(")", symbolToken.Symbol);

           #endregion

           Assert.Equal(typeof(StopToken), ((StopToken)stream.ReadNextToken()).GetType());
       }

       finally
       {
           File.Delete(filePath);
       }
   }

   [Fact]
   public void TestSceneFile()
   {
       const string content = "abc   \nd\nef";

       string filePath = Path.GetTempFileName();
       File.WriteAllText(filePath, content);

       try
       {
           InputStream stream = new InputStream(filePath);

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
   }*/
}