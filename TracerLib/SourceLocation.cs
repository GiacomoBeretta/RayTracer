// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// Represents a position within a source file.
/// Line and column start from 0.
/// </summary>
public struct SourceLocation
{
    public string filePath;
    public int line;
    public int column;

    /// <summary>
    /// Initializes a new <see cref="SourceLocation"/> for the specified file,
    /// with line and column set to 0.
    /// </summary>
    /// <param name="filePath">The path of the source file.</param>
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
        return "Source Location: " + filePath + ", line " + line + ", column " + column;
    }
}