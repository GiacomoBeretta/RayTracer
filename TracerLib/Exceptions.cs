// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// The class <c>InvalidPfmFileFormat</c> inherit from the FormatException's class and is used in the error management during the reading/writing of a file.pfm
/// </summary>
public class InvalidPfmFileFormat : FormatException
{
    public InvalidPfmFileFormat(string errorMessage) : base(errorMessage)
    {
    }

    public InvalidPfmFileFormat(string message, Exception inner) : base(message, inner)
    {
    }
}

//e come si stampa poi la location?
public class GrammarError : Exception
{
    public GrammarError(string message) : base(message)
    {
    }

    public GrammarError(string message, Exception inner) : base(message, inner)
    {
    }

    public GrammarError(SourceLocation location, string message) : base($"Grammar Error at ${location}: {message}")
    {
    }

    public GrammarError(SourceLocation location, string message, Exception inner) : base(
        $"Grammar Error at ${location}: {message}", inner)
    {
    }

    /*
    public GrammarError(string message, SourceLocation location) : base(message)
    {
        this.location = location;
    }
    public GrammarError(string message, SourceLocation location, Exception inner) : base(message, inner)
    {
        this.location = location;
    }*/
}

/* public class ZeroDivision : ArithmeticException
{
    public ZeroDivision(string error) : base(error){}
}

public class Calculator{
    public static float Divide(float num, float den)
    {
        if (den == 0f)
        {
            throw new ZeroDivision("Impossibile dividere per zero!");
        }

        return num / den;
    }
    }
    */