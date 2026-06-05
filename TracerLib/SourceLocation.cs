namespace TracerLib;

/// <summary>
/// This struct specify a position in a file.
/// line and column start from 0
/// </summary>
public struct SourceLocation
{
    private string filePath;
    //private int indexName;
    public int line;
    public int column;

    public SourceLocation(string filePath)
    {
        this.filePath = filePath;
        this.line = 0;
        this.column = 0;
    }
    
    public SourceLocation(string filePath, int line, int column)
    {
        this.filePath = filePath;
        this.line = line;
        this.column = column;
    }

    public override string ToString()
    {
        return "Source Location: "+ filePath + ", line " + line + ", column " + column;
    }
}