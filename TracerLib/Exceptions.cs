namespace TracerLib;
//namespace Exception;

public class InvalidPfmFileFormat : FormatException
{
    public InvalidPfmFileFormat() : base() { }
    public InvalidPfmFileFormat(string errorMessage) : base (errorMessage) { }
    public InvalidPfmFileFormat(string message, Exception inner) : base(message, inner) { }
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