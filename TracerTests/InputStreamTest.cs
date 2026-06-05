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
        SourceLocation location = new SourceLocation(filePath);
        InputStream str = new InputStream(filePath);

        Assert.Equal(location, str.Location);
        Assert.Equal(location, str.SavedLocation);
        Assert.Null(str.SavedChar);
        Assert.Equal(8, str.Tabulations);
        Assert.Null(str.SavedToken);

        File.Delete(filePath);
    }

    [Fact]
    public void TestUpdateLocation()
    {
        string filePath = Path.GetTempFileName();
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

        File.Delete(filePath);
    }

    [Fact]
    public void TestReadChar_UnreadChar()
    {
        const string content = "abcde";
        string filePath = Path.GetTempFileName();
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

        File.Delete(filePath);
    }

    //test skipLIne

    //test skipWhiteSPaces

    //test parseStringTOken
    //test parsefloat

    //test parseKEYWORDiDENTIFIER

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

    [Fact]
    public void TestParseStringToken()
    {
        const string content = "Hello, World!\"";
        const string fail = "Hello, World!";

        var file1 = Path.GetTempFileName();
        File.WriteAllText(file1, content);

        var file2 = Path.GetTempFileName();
        File.WriteAllText(file2, fail);

        try
        {
            var stream1 = new InputStream(file1);
            var st1 = stream1._ParseStringToken(stream1.Location);

            var stream2 = new InputStream(file2);

            Assert.Equal("Hello, World!", st1.String);
            Assert.Throws<GrammarError>(() => stream2._ParseStringToken(stream2.Location));
        }
        finally
        {
            File.Delete(file1);
            File.Delete(file2);
        }
    }
}