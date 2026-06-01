namespace TracerLib;

/// <summary>
/// This struct specify a position in a file.
/// line and column start from 0
/// </summary>
public struct SourceLocation
{
    private string fileName;
    //private int indexName;
    public int line;
    public int column;

    public SourceLocation(string fileName, int line, int column)
    {
        this.fileName = fileName;
        this.line = line;
        this.column = column;
    }

    public override string ToString()
    {
        return "Source Location: "+ fileName + ", line " + line + ", column " + column;
    }
}

/*public class Token
{
    SourceLocation location;

    public Token(SourceLocation location)
    {
        this.location = location;
    }
    
    public Token(string fileName, int line, int column)
    {
        this.location = new SourceLocation(fileName, line, column);
    }
}*/