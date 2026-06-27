// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// Thrown when a PFM file has an invalid format.
/// </summary>
public class InvalidPfmFileFormatException : FormatException
{
    public InvalidPfmFileFormatException(string errorMessage) : base(errorMessage)
    {
    }

    public InvalidPfmFileFormatException(string message, Exception inner) : base(message, inner)
    {
    }
}

/// <summary>
/// Thrown when a syntax error is encountered while parsing a scene file.
/// </summary>
public class SceneSyntaxException : Exception
{
    /// <summary>
    /// The location of the error (file name, column and row). See <see cref="SourceLocation"/>.
    /// </summary>
    public SourceLocation Location { get; }

    public SceneSyntaxException(SourceLocation location, string message) : base($"Grammar Error at ${location}: {message}")
    {
        Location = location;
    }

    public SceneSyntaxException(SourceLocation location, string message, Exception inner) : base(
        $"Grammar Error at ${location}: {message}", inner)
    {
        Location = location;
    }
}