// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// The class <see cref="InvalidPfmFileFormatException"/> inherit from the FormatException's class and is used in the error management during the reading/writing of a file.pfm
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
/// The class <see cref="GrammarException"/> is used in the error management during the reading of a scene file
/// </summary>
public class GrammarException : Exception
{
    SourceLocation Location;

    public GrammarException(SourceLocation location, string message) : base($"Grammar Error at ${location}: {message}")
    {
        Location = location;
    }

    public GrammarException(SourceLocation location, string message, Exception inner) : base(
        $"Grammar Error at ${location}: {message}", inner)
    {
        Location = location;
    }
}