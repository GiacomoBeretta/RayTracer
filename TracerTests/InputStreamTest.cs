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
    public void TestSceneFile()
    {
        const string content = "abc   \nd\nef";

        var file = Path.GetTempFileName();
        File.WriteAllText(file, content);

        try
        {
            var stream = new InputStream(file);
            
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
            File.Delete(file);
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
            Assert.Throws<GrammarError> ( () => stream2._ParseStringToken(stream2.Location));
        }
        finally
        {
            File.Delete(file1);
            File.Delete(file2);
        }
    }
}
}